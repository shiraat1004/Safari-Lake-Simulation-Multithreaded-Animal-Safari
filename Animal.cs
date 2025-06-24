using System;
using System.Drawing;
using System.Threading; 
using System.Windows.Forms;

namespace Safari
{
    public class Animal
    {
        public string Type { get; protected set; }
        public PictureBox AnimalPictureBox { get; set; }
        public int CapacityTaken { get; protected set; }
        public Point CurrentLocation { get; set; } 
        public Point TargetLocation { get; set; } 
        public Size Size { get; protected set; } = new Size(60, 60);

        protected const double StandardDeviation = 0.2;

        public Lake TargetLake { get; set; } 

        public Animal()
        {
            CapacityTaken = 1; 
        }

        public virtual int GetDrinkTime() => 0; 
        public virtual int GetArrivalTime() => 0; 

        public void StartLifeCycle()
        {
            

            AnimateToEntry(this.TargetLake.GetEntryLocation());
            Thread.Sleep(Randomizer.NextGaussian(GetArrivalTime(), GetArrivalTime() * StandardDeviation));

          
            TargetLake.AddAnimalToQueue(this);
        }

        private void AnimateToEntry(Point targetLocation)
        {
            if (AnimalPictureBox == null || AnimalPictureBox.IsDisposed)
            {
                Console.WriteLine($"DEBUG: AnimalPictureBox for {Type} is null or already disposed during AnimateToEntry. Skipping.");
                return;
            }

            AnimalPictureBox.Invoke((MethodInvoker)delegate
            {
                if (AnimalPictureBox.IsDisposed)
                {
                    Console.WriteLine($"DEBUG: AnimalPictureBox for {Type} was disposed just before its UI update within Invoke. Skipping.");
                    return;
                }

                AnimalPictureBox.Visible = true; 
                AnimalPictureBox.Location = targetLocation;
            });
        }
    }

    public class Flamingo : Animal
    {
        public Flamingo()
        {
            Type = "F";
            CapacityTaken = 1;
            Size = new Size(80, 80); 
        }
        public override int GetDrinkTime() => Randomizer.NextGaussian(3500, 3500 * StandardDeviation);
        public override int  GetArrivalTime() => Randomizer.NextGaussian(2000, 2000 * StandardDeviation);
    }

    public class Zebra : Animal
    {
        public Zebra()
        {
            Type = "Z";
            CapacityTaken = 2;
            Size = new Size(80, 80);
        }
        public override int GetDrinkTime() => Randomizer.NextGaussian(5000, 5000 * StandardDeviation);
        public override int GetArrivalTime() => Randomizer.NextGaussian(3000, 3000 * StandardDeviation);
    }

    public class Hippo : Animal
    {
        public Hippo()
        {
            Type = "H";
            CapacityTaken = int.MaxValue;
            Size = new Size(80, 80); 
        }
        public override int GetDrinkTime() => Randomizer.NextGaussian(5000, 5000 * StandardDeviation);
        public override int GetArrivalTime() => Randomizer.NextGaussian(10000, 10000 * StandardDeviation);
    }
}
