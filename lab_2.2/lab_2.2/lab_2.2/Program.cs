using System;
using System.Collections.Generic;

class Processor
{
    private Queue<Process> queue = new Queue<Process>();
    private bool processor1Busy = false;
    private bool processor2Busy = false;
    private int maxQueueLength = 0;
    private int discardedCount1 = 0;
    private int interruptedCount2 = 0;

    public void GenerateProcess(int threadId)
    {
        Process process = new Process(threadId);
        lock (this)
        {
            if (threadId == 1)
            {
                if (processor1Busy)
                {
                    if (processor2Busy)
                    {
                        discardedCount1++;
                        Console.WriteLine("Process 1 discarded.");
                    }
                    else
                    {
                        processor2Busy = true;
                        Console.WriteLine("Process 1 sent to Processor 2.");
                    }
                }
                else
                {
                    processor1Busy = true;
                    Console.WriteLine("Process 1 started on Processor 1.");
                }
            }
            else if (threadId == 2)
            {
                if (processor2Busy)
                {
                    if (processor1Busy)
                    {
                        interruptedCount2++;
                        Console.WriteLine("Process 2 interrupted.");
                    }
                    else
                    {
                        processor1Busy = true;
                        Console.WriteLine("Process 2 sent to Processor 1.");
                    }
                }
                else
                {
                    processor2Busy = true;
                    Console.WriteLine("Process 2 started on Processor 2.");
                }
            }

            if (queue.Count > maxQueueLength)
            {
                maxQueueLength = queue.Count;
            }
        }
    }

    public void ProcessCompleted(int threadId)
    {
        lock (this)
        {
            if (threadId == 1)
            {
                processor1Busy = false;
            }
            else if (threadId == 2)
            {
                processor2Busy = false;
            }

            if (queue.Count > 0)
            {
                Process nextProcess = queue.Dequeue();
                GenerateProcess(nextProcess.GetThreadId());
            }
        }
    }

    public int GetMaxQueueLength()
    {
        return maxQueueLength;
    }

    public int GetDiscardedCount1()
    {
        return discardedCount1;
    }

    public int GetInterruptedCount2()
    {
        return interruptedCount2;
    }
}

class Process
{
    private int threadId;

    public Process(int threadId)
    {
        this.threadId = threadId;
    }

    public int GetThreadId()
    {
        return threadId;
    }
}

class MainClass
{
    public static void Main(string[] args)
    {
        Processor processor = new Processor();
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int threadId = random.Next(1, 3);
            processor.GenerateProcess(threadId);
        }

        Console.WriteLine("Max Queue Length: " + processor.GetMaxQueueLength());
        Console.WriteLine("Percentage of Discarded Processes for Thread 1: " + (processor.GetDiscardedCount1() / 100.0) * 100 + "%");
        Console.WriteLine("Percentage of Interrupted Processes for Thread 2: " + (processor.GetInterruptedCount2() / 100.0) * 100 + "%");
    }
}
