using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Collections.Generic;

namespace Kill_exp_2
{

    class Program
    {

        static void Main(string[] args)
        {
            String param1, param2, param3;
            List<Process> allProcesses;
            List<Process> zumbieProcesses;
            List<Process> namedProcesses;
            // --------------- entrada de parâmetros ------------------ //
            if (args.Length < 2)
            {
                MenuHelp();
            }
            else if (args.Length < 3)
            {
                param1 = args[0];
                param2 = args[1];
                if (String.Compare(param1, "list", true) == 0 && String.Compare(param2, "all", true) == 0)
                {
                    allProcesses = new List<Process>(Process.GetProcesses());
                    Console.WriteLine("\n" + allProcesses.Count + " process(es) found!");
                    foreach (Process proc in allProcesses)
                    {
                        Console.WriteLine("[{0}] - PID: {1} ", proc.ProcessName, proc.Id);
                    }
                }
                else if (String.Compare(param1, "list", true) == 0) // second parameter must be a process name
                {
                    namedProcesses = new List<Process>(Process.GetProcessesByName(param2));
                    Console.WriteLine("\n" + namedProcesses.Count + " process(es) found!");
                    if (namedProcesses.Count > 0)
                    {
                        foreach (Process proc in namedProcesses)
                        {
                            try
                            {
                                Process p = GetParentId(proc.Id);
                                Console.WriteLine("{0} - PID: {1} -> parent[{2} - pPID: {3} - pageSize: {4}]", proc.ProcessName, proc.Id, p.ProcessName, p.Id, proc.PagedMemorySize64);
                            }
                            catch (ArgumentException e)
                            {
                                Console.WriteLine("{0} - PID: {1}***Zumbie -> parent[none - pPID: none]: [{2}]", proc.ProcessName, proc.Id, e.Message);
                            }
                        }
                    }
                }
                else
                {
                    MenuHelp();
                }
            }
            else if (args.Length < 4)
            {
                param1 = args[0];
                param2 = args[1];
                param3 = args[2];
                if (String.Compare(param1, "kill", true) == 0 && String.Compare(param3, "zumbie", true) == 0) // second parameter must be a process name
                {
                    namedProcesses = new List<Process>(Process.GetProcessesByName(param2));
                    zumbieProcesses = new List<Process>();
                    Console.WriteLine("\n" + namedProcesses.Count + " process(es) found!");
                    if (namedProcesses.Count > 0)
                    {
                        foreach (Process proc in namedProcesses)
                        {
                            try
                            {
                                Process p = GetParentId(proc.Id);
                                Console.WriteLine("{0} - PID: {1} -> parent[{2} - pPID: {3} - pageSize: {4}]", proc.ProcessName, proc.Id, p.ProcessName, p.Id, proc.PagedMemorySize64);
                            }
                            catch (ArgumentException e)
                            {
                                Console.WriteLine("{0} - PID: {1}***Zumbie -> parent[none - pPID: none]: [{2}]", proc.ProcessName, proc.Id, e.Message);
                                zumbieProcesses.Add(proc);
                            }
                        }
                    }
                    // kill "zumbie" processes
                    foreach (Process proc in zumbieProcesses)
                    {
                        proc.Kill();
                    }
                    // delay, to give Windows time to flush process structure (begin)
                    Thread.Sleep(1000);
                    // end delay
                    Process[] verify = Process.GetProcessesByName(param2);
                    if (verify.Length > 0)
                    {
                        Console.WriteLine("\nProcess(es) NOT killed: " + verify.Length);
                        foreach (Process proc2 in verify)
                        {
                            Console.WriteLine("{0} - ID: {1} ", proc2.ProcessName, proc2.Id);
                        }
                        Console.WriteLine("\nProcess(es) killed: " + (namedProcesses.Count - verify.Length));
                        foreach (Process pkilled in zumbieProcesses)
                        {
                            Console.WriteLine("{0} - ID: {1} ", pkilled.ProcessName, pkilled.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nProcess(es) killed: " + (namedProcesses.Count - verify.Length));
                    }
                }
                else if (String.Compare(param1, "kill", true) == 0 && // second parameter must be a process name
                         String.Compare(param3, "all", true) == 0)
                {
                    namedProcesses = new List<Process>(Process.GetProcessesByName(param2));
                    Console.WriteLine("\n" + namedProcesses.Count + " process(es) found!");
                    if (namedProcesses.Count > 0)
                    {
                        foreach (Process proc in namedProcesses)
                        {
                            try
                            {
                                Process p = GetParentId(proc.Id);
                                Console.WriteLine("{0} - PID: {1} -> parent[{2} - pPID: {3}]", proc.ProcessName, proc.Id, p.ProcessName, p.Id);
                            }
                            catch (ArgumentException e)
                            {
                                Console.WriteLine("{0} - PID: {1}***Zumbie -> parent[none - pPID: none]: [{2}]", proc.ProcessName, proc.Id, e.Message);
                            }
                        }
                    }
                    // kill "zumbie" processes
                    foreach (Process proc in namedProcesses)
                    {
                        proc.Kill();
                    }
                    // delay, to give Windows time to flush process structure (begin)
                    Thread.Sleep(1000);
                    // end delay
                    Process[] verify = Process.GetProcessesByName(param2);
                    if (verify.Length > 0)
                    {
                        Console.WriteLine("\nProcess(es) NOT killed: " + verify.Length);
                        foreach (Process proc2 in verify)
                        {
                            Console.WriteLine("{0} - ID: {1} ", proc2.ProcessName, proc2.Id);
                        }
                        Console.WriteLine("\nProcess(es) killed: " + (namedProcesses.Count - verify.Length));
                        foreach (Process pkilled in namedProcesses)
                        {
                            Console.WriteLine("{0} - ID: {1} ", pkilled.ProcessName, pkilled.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nProcess(es) killed: " + (namedProcesses.Count - verify.Length));
                    }
                }else if(String.Compare(param1, "list", true) == 0 && String.Compare(param2, "paged", true) == 0 && String.Compare(param3, "memory", true) == 0)
                {
                    allProcesses = new List<Process>(Process.GetProcesses());
                    Console.WriteLine("\n" + allProcesses.Count + " process(es) found!");
                    foreach (Process proc in allProcesses)
                    {
                        Console.WriteLine("[{0}] - PID: {1} - Memória paginada: {2}", proc.ProcessName, proc.Id, proc.PagedMemorySize64);
                    }
                }else if (String.Compare(param1, "list", true) == 0 && String.Compare(param2, "system", true) == 0 && String.Compare(param3, "memory", true) == 0)
                {
                    allProcesses = new List<Process>(Process.GetProcesses());
                    Console.WriteLine("\n" + allProcesses.Count + " process(es) found!");
                    foreach (Process proc in allProcesses)
                    {
                        Console.WriteLine("[{0}] - PID: {1} - Memória de sistema paginada: {2}", proc.ProcessName, proc.Id, proc.PagedSystemMemorySize64);
                    }
                }else if(String.Compare(param1, "list", true) == 0 && String.Compare(param3, "threads", true) == 0)
                {
                    namedProcesses = new List<Process>(Process.GetProcessesByName(param2));
                    Console.WriteLine("\n" + namedProcesses.Count + " process(es) found!");
                    if (namedProcesses.Count > 0)
                    {
                        foreach (Process proc in namedProcesses)
                        {
                            Console.WriteLine("[{0}] - ", proc.ProcessName);
                            ProcessThreadCollection t = proc.Threads;
                            foreach (ProcessThread th in t)
                            {
                                Console.WriteLine("Thread ID: {0} - State: {1} - {2}", th.Id, th.ThreadState, th.ToString());
                            }
                        }
                    }
                }
                else if (String.Compare(param1, "list", true) == 0 && String.Compare(param2, "virtual", true) == 0 && String.Compare(param3, "memory", true) == 0)
                {
                    allProcesses = new List<Process>(Process.GetProcesses());
                    Console.WriteLine("\n" + allProcesses.Count + " process(es) found!");
                    foreach (Process proc in allProcesses)
                    {
                        try
                        {
                            Console.WriteLine("[{0}] - Tempo Proc. Total: {1} - Memória virtual: {2}", proc.ProcessName, proc.TotalProcessorTime, proc.VirtualMemorySize64);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine("{0} - Erro:", proc.ProcessName, e.Message);
                        }
                    }
                }
                else
                {
                    MenuHelp();
                }
            }
            else
            {
                MenuHelp();
            }
        }
        static Process GetParentId(int PID) 
        {
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", PID);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();
            var queryObj = results.Current;
            var parentId = (uint)queryObj["ParentProcessId"];
            Process parentProcess = Process.GetProcessById((int)parentId);
            return parentProcess;
        }
        static void MenuHelp()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("\n > CleanProc list  all                    : To list all processes");
            Console.WriteLine("\n > CleanProc list  <process_name>         : To list processes named <process_name>");
            Console.WriteLine("\n > CleanProc kill  <process_name>  zumbie : To kill processes named <process_name> that are zumbies ...");
            Console.WriteLine("\n > CleanProc kill  <process_name>  all    : To kill ALL processes named <process_name>");
            //Environment.Exit(1);
        }
    }
}
