using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Safari
{
    public class Lake
    {
        public Panel panelLake;
        public Panel panelQueue;
        private int maxCapacitySlots;
        private LinkedList<Animal> waitLine = new LinkedList<Animal>();
        private Animal[] lakeSlots;
        private int currentOccupiedSlots = 0;
        private bool isHippoPresent = false;
        private SemaphoreSlim lakeCapacitySemaphore;
        private readonly object lakeLock = new object();
        private readonly object queueLock = new object();
        private ManualResetEventSlim hippoArrivalEvacuateEvent = new ManualResetEventSlim(false);
        private MainForm mainForm;


        public Lake(int maxCapacitySlots, Panel panelLake, Panel panelQueue, MainForm mainForm)
        {
            this.maxCapacitySlots = maxCapacitySlots;
            this.panelLake = panelLake;
            this.panelQueue = panelQueue;
            lakeSlots = new Animal[maxCapacitySlots];
            lakeCapacitySemaphore = new SemaphoreSlim(maxCapacitySlots, maxCapacitySlots);
            Thread queueProcessorThread = new Thread(() => ProcessQueue());
            queueProcessorThread.IsBackground = true;
            queueProcessorThread.Start();
            this.mainForm = mainForm;
        }

        public Point GetEntryLocation()
        {
            if (panelQueue != null)
            {
                return new Point(panelQueue.Location.X + panelQueue.Width / 2, panelQueue.Location.Y - 50);
            }
            return new Point(0, 0);
        }

        public void AddAnimalToQueue(Animal animal)
        {
            lock (queueLock)
            {
                if (animal.Type == "H")
                {
                    waitLine.AddFirst(animal);
                }
                else
                {
                    waitLine.AddLast(animal);
                }
                Monitor.Pulse(queueLock);
            }
            DisplayQueueAnimals();
        }

        private void ProcessQueue()
        {
            while (true)
            {
                Animal currentAnimalToProcess = null;

                lock (queueLock)
                {
                    while (waitLine.Count == 0)
                    {
                        Monitor.Wait(queueLock);
                    }
                    currentAnimalToProcess = waitLine.First?.Value;
                }

                if (currentAnimalToProcess != null)
                {
                    bool enteredLake = TryEnterLake(currentAnimalToProcess);

                    if (enteredLake)
                    {
                        lock (queueLock)
                        {
                            if (waitLine.Count > 0 && waitLine.First?.Value == currentAnimalToProcess)
                            {
                                waitLine.RemoveFirst();
                            }
                        }
                        DisplayQueueAnimals();
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                Thread.Sleep(10);
            }
        }

        private bool TryEnterLake(Animal animal)
        {
            if (animal.Type == "H")
            {

                lock (lakeLock)
                {
                    isHippoPresent = true;

                    if (currentOccupiedSlots > 0)
                    {
                        for (int i = 0; i < maxCapacitySlots; i++)
                        {
                            Animal animalInSlot = lakeSlots[i];
                            if (animalInSlot != null)
                            {
                                lakeSlots[i] = null;

                                if (animalInSlot.AnimalPictureBox != null && !animalInSlot.AnimalPictureBox.IsDisposed)
                                {
                                    panelLake.Invoke((MethodInvoker)delegate
                                    {
                                        if (animalInSlot.AnimalPictureBox != null && !animalInSlot.AnimalPictureBox.IsDisposed)
                                        {
                                            panelLake.Controls.Remove(animalInSlot.AnimalPictureBox);
                                            animalInSlot.AnimalPictureBox.Dispose();
                                            animalInSlot.AnimalPictureBox = null; // Mark as removed
                                        }
                                    });
                                }
                            }
                        }
                        currentOccupiedSlots = 0;
                    }

                    lakeCapacitySemaphore = new SemaphoreSlim(0, maxCapacitySlots);

                    if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                    {
                        panelLake.Invoke((MethodInvoker)delegate
                        {
                            if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                            {
                                animal.AnimalPictureBox.Parent = panelLake;
                                animal.AnimalPictureBox.BringToFront(); 
                            }
                        });
                    }

                    Array.Clear(lakeSlots, 0, maxCapacitySlots);
                    lakeSlots[0] = animal; 
                    currentOccupiedSlots = maxCapacitySlots;
                    DisplayLakeAnimals(); 

                    Monitor.PulseAll(lakeLock);
                } 

                Thread hippoDrinkingThread = new Thread(() => HippoDrinksAndLeaves(animal));
                hippoDrinkingThread.IsBackground = true;
                hippoDrinkingThread.Start();

                return true;
            }

            lock (lakeLock)
            {
                if (isHippoPresent)
                {
                    return false;
                }

                bool acquiredAllSlots = true;
                for (int i = 0; i < animal.CapacityTaken; i++)
                {
                    if (!lakeCapacitySemaphore.Wait(0)) 
                    {
                        lakeCapacitySemaphore.Release(i);
                        acquiredAllSlots = false;
                        break;
                    }
                }

                if (!acquiredAllSlots)
                {
                    return false;
                }

                try
                {
                    int requiredSlots = animal.CapacityTaken;
                    List<int> potentialSlots = new List<int>();

                    if (animal.Type == "F")
                    {
                        int foundSlot = -1;
                        for (int i = 0; i < maxCapacitySlots; i++)
                        {
                            if (lakeSlots[i] == null)
                            {
                                if ((i > 0 && lakeSlots[i - 1]?.Type == "F") || (i < maxCapacitySlots - 1 && lakeSlots[i + 1]?.Type == "F"))
                                {
                                    foundSlot = i;
                                    break;
                                }
                            }
                        }
                        if (foundSlot == -1)
                        {
                            for (int i = 0; i < maxCapacitySlots; i++)
                            {
                                if (lakeSlots[i] == null)
                                {
                                    foundSlot = i;
                                    break;
                                }
                            }
                        }

                        if (foundSlot != -1)
                        {
                            potentialSlots.Add(foundSlot);
                        }
                    }
                    else if (animal.Type == "Z")
                    {
                        for (int i = 0; i <= maxCapacitySlots - requiredSlots; i++)
                        {
                            bool canFit = true;
                            for (int j = 0; j < requiredSlots; j++)
                            {
                                if (lakeSlots[i + j] != null)
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                            if (canFit)
                            {
                                for (int j = 0; j < requiredSlots; j++)
                                {
                                    potentialSlots.Add(i + j);
                                }
                                break;
                            }
                        }
                    }

                    if (potentialSlots.Count == requiredSlots)
                    {
                        if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                        {
                            panelLake.Invoke((MethodInvoker)delegate
                            {
                                if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                                {
                                    animal.AnimalPictureBox.Parent = panelLake;
                                    animal.AnimalPictureBox.BringToFront();
                                }
                            });
                        }

                        foreach (int slotIndex in potentialSlots)
                        {
                            lakeSlots[slotIndex] = animal;
                        }
                        currentOccupiedSlots += requiredSlots;

                        DisplayLakeAnimals();

                        Thread drinkingThread = new Thread(() => AnimalDrinksAndLeaves(animal, potentialSlots.ToArray()));
                        drinkingThread.IsBackground = true;
                        drinkingThread.Start();

                        return true;
                    }
                    else
                    {
                        lakeCapacitySemaphore.Release(requiredSlots);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    lakeCapacitySemaphore.Release(animal.CapacityTaken);
                    throw;
                }
            }
        }

        private void AnimalDrinksAndLeaves(Animal animal, int[] occupiedSlots)
        {
            int totalDrinkTime = animal.GetDrinkTime();

            Thread.Sleep(totalDrinkTime);


            lock (lakeLock)
            {

                bool wasActuallyInLake = false;
                foreach (int slotIndex in occupiedSlots)
                {
                    if (slotIndex < maxCapacitySlots && lakeSlots[slotIndex] == animal)
                    {
                        wasActuallyInLake = true;
                        break;
                    }
                }

                if (wasActuallyInLake)
                {
                    foreach (int slotIndex in occupiedSlots)
                    {
                        if (slotIndex < maxCapacitySlots && lakeSlots[slotIndex] == animal)
                        {
                            lakeSlots[slotIndex] = null;
                        }
                    }
                    currentOccupiedSlots -= animal.CapacityTaken;

                    try
                    {
                        lakeCapacitySemaphore.Release(animal.CapacityTaken);
                    }
                    catch (SemaphoreFullException)
                    {
                    }
                }
                else
                {
                }

                if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                {
                    panelLake.Invoke((MethodInvoker)delegate
                    {
                        if (animal.AnimalPictureBox.Parent == panelLake && !animal.AnimalPictureBox.IsDisposed)
                        {
                            panelLake.Controls.Remove(animal.AnimalPictureBox);
                            animal.AnimalPictureBox.Dispose();
                            animal.AnimalPictureBox = null;
                        }
                    });
                }

                DisplayLakeAnimals();
                Monitor.PulseAll(lakeLock);
            }
        }

        private void HippoDrinksAndLeaves(Animal animal)
        {
            Thread.Sleep(animal.GetDrinkTime());

            lock (lakeLock)
            {

                if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                {
                    panelLake.Invoke((MethodInvoker)delegate
                    {
                        if (animal.AnimalPictureBox.Parent == panelLake && !animal.AnimalPictureBox.IsDisposed)
                        {
                            panelLake.Controls.Remove(animal.AnimalPictureBox);
                            animal.AnimalPictureBox.Dispose();
                            animal.AnimalPictureBox = null;
                        }
                    });
                }

                lakeSlots[0] = null;
                isHippoPresent = false;
                currentOccupiedSlots = 0;
                DisplayLakeAnimals();
                Monitor.PulseAll(lakeLock);
            }

            lakeCapacitySemaphore = new SemaphoreSlim(maxCapacitySlots, maxCapacitySlots);

            hippoArrivalEvacuateEvent.Reset();
        }

   

        private void DisplayQueueAnimals()
        {
       
                if (mainForm != null && mainForm.isClosing) return;

                if (panelQueue == null || panelQueue.IsDisposed || !panelQueue.IsHandleCreated)
                    return;

                if (panelQueue.InvokeRequired)
                {
                    panelQueue.Invoke(new MethodInvoker(RenderQueueContents));
                }
                else
                {
                    RenderQueueContents();
                }
            
   
        }

        private void RenderQueueContents()
        {
            if (mainForm != null && mainForm.isClosing) return;
            if (panelQueue == null || panelQueue.IsDisposed || !panelQueue.IsHandleCreated)
                return;

            int animalYOffset = 0;
            int horizontalCenterOffset = panelQueue.Width / 2; 

            panelQueue.Controls.Clear(); 

            List<Animal> queueSnapshot;
            lock (queueLock)
            {
                queueSnapshot = new List<Animal>(waitLine);
            }

            foreach (var animal in queueSnapshot)
            {
                if (animal.AnimalPictureBox != null && !animal.AnimalPictureBox.IsDisposed)
                {
              
                    if (animal.AnimalPictureBox.Parent != panelQueue)
                    {
                        animal.AnimalPictureBox.Parent = panelQueue;
                    }

                    animal.AnimalPictureBox.Location = new Point(
                        horizontalCenterOffset - (animal.AnimalPictureBox.Width / 2),
                        animalYOffset);

            
                    if (!panelQueue.Controls.Contains(animal.AnimalPictureBox))
                    {
                        panelQueue.Controls.Add(animal.AnimalPictureBox);
                    }

                    animal.AnimalPictureBox.Visible = true;
                    animal.AnimalPictureBox.BringToFront();

                
                    animalYOffset += animal.AnimalPictureBox.Height + 5; 
                }
            }
        }

        private void DisplayLakeAnimals()
        {
            if (mainForm != null && mainForm.isClosing) return;

            Animal[] animalsInSnapshot;
            lock (lakeLock)
            {
                animalsInSnapshot = (Animal[])lakeSlots.Clone();
            }

            if (panelLake != null && !panelLake.IsDisposed && panelLake.IsHandleCreated && panelLake.InvokeRequired)
            {
                panelLake.Invoke(new MethodInvoker(() => InternalDisplayLakeAnimals(animalsInSnapshot)));
                return;
            }
            else
            {
                InternalDisplayLakeAnimals(animalsInSnapshot);
            }
        }

        private void InternalDisplayLakeAnimals(Animal[] animalsInSnapshot)
        {
            if (mainForm != null && mainForm.isClosing) return;
            if (panelLake == null || panelLake.IsDisposed || !panelLake.IsHandleCreated) return;

            List<PictureBox> currentPanelPictureBoxes = new List<PictureBox>();
            foreach (Control control in panelLake.Controls)
            {
                if (control is PictureBox pb)
                {
                    currentPanelPictureBoxes.Add(pb);
                }
            }

            HashSet<PictureBox> pictureBoxesToKeep = new HashSet<PictureBox>();

            int baseSlotWidth = panelLake.Width / maxCapacitySlots;

            for (int i = 0; i < maxCapacitySlots; i++)
            {
                Animal animal = animalsInSnapshot[i];
                if (animal != null)
                {
                    if (animal.AnimalPictureBox == null || animal.AnimalPictureBox.IsDisposed)
                    {
                        continue;
                    }

                    Point targetPos;
                    if (animal.Type == "Z")
                    {
                        targetPos = new Point(
                            (int)(i * baseSlotWidth + (baseSlotWidth * 2 - animal.Size.Width) / 2.0),
                            panelLake.Height - animal.Size.Height - 10);
                    }
                    else if (animal.Type == "H")
                    {
                        targetPos = new Point(
                            (panelLake.Width - animal.Size.Width) / 2,
                            (panelLake.Height - animal.Size.Height) / 2);
                    }
                    else
                    {
                        targetPos = new Point(
                            (int)(i * baseSlotWidth + (baseSlotWidth - animal.Size.Width) / 2.0),
                            panelLake.Height - animal.Size.Height - 10);
                    }

                    animal.AnimalPictureBox.Size = animal.Size;
                    animal.AnimalPictureBox.Location = targetPos;

                    
                    if (animal.AnimalPictureBox.Parent != panelLake)
                    {
                        animal.AnimalPictureBox.Parent = panelLake;
                    }

                    if (!panelLake.Controls.Contains(animal.AnimalPictureBox))
                    {
                        panelLake.Controls.Add(animal.AnimalPictureBox);
                    }

                    animal.AnimalPictureBox.Visible = true;
                    animal.AnimalPictureBox.BringToFront();

                    pictureBoxesToKeep.Add(animal.AnimalPictureBox);
                }
            }

            foreach (PictureBox pb in currentPanelPictureBoxes)
            {
                if (!pictureBoxesToKeep.Contains(pb))
                {
                    if (panelLake.Controls.Contains(pb))
                    {
                        panelLake.Controls.Remove(pb);
                    }
                    if (!pb.IsDisposed)
                    {
                        pb.Dispose();
                    }
                }
            }
        }
    }
}