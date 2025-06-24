# Safari-Lake-Simulation-Multithreaded-Animal-Safari
Colorful Windows Forms demo of Semaphore / Mutex coordination: flamingos, zebras and hippos queue to drink from a lake.

Safari-Lake-Simulation is a C# WinForms project that visualizes classic concurrency concepts.

Actors as threads – each Animal (flamingo, zebra, hippo) runs on its own thread.

Lake constraints – flamingos & zebras can share; hippos require exclusive access (implemented with SemaphoreSlim + Mutex).

Queue & animation – threads wait in a visible queue, then move into the lake area using PictureBox animation.

Random arrivals – Randomizer spawns animals at stochastic intervals for a lively demo.

Teaching aid – great for lectures or code reviews on synchronization primitives and UI thread-invocation patterns.
