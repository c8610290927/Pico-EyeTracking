using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Neurorehab.Scripts.Utilities.Logger
{
    /// <summary>
    /// Adds Logging capability to the Scene. To use it, call the <see cref="WriteLine"/> method from the singleton reference (<see cref="Instance"/>)
    /// </summary>
    public class Logger : MonoBehaviour
    {
        public static bool Instantiated;

        /// <summary>
        /// Indicates if the Logger is logging or not.
        /// </summary>
        public bool Logging = true;

        /// <summary>
        /// The singleton reference
        /// </summary>
        public static Logger Instance;
        /// <summary>
        /// Locks the access to the file so it can be used in multhread scenarios.
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// The start time for the logging.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// A list of all logging files existing in the corrent executiong.
        /// </summary>
        private List<string> _paths;

        private void Awake()
        {
            _paths = new List<string>();
            _startTime = DateTime.Now;
            Instance = this;
            Instantiated = true;
        }

        /// <summary>
        /// Pressing Space will add a marker at the current time.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                WriteLine("MARKER", "ALL");
            if (Input.GetKeyDown(KeyCode.F12))
                StartLogging();
        }

        /// <summary>
        /// Restarts the logging process. Updates the <see cref="_startTime"/> variable and creates new files for the logging.
        /// </summary>
        private void StartLogging()
        {
            lock (_lock)
            {
                Logging = true;
                _paths.Clear();
                _startTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Writes the received line to the logging file.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fileName">The file where this is going to be logged</param>
        public void WriteLine(string line, string fileName)
        {
            if (Logging == false) return;

            line = DateTime.Now - _startTime + "-" + line;

            lock (_lock)
            {
                if (fileName == "ALL")
                    foreach (var path in _paths)
                        AppendLineToFile(GetPath(path), line);
                else
                    AppendLineToFile(GetPath(fileName), line);
            }
        }

        /// <summary>
        /// Appends the line to the file in the received path.
        /// </summary>
        /// <param name="path"> The complete path to the file. </param>
        /// <param name="line"> The line to be appended. </param>
        private void AppendLineToFile(string path, string line)
        {
            using (var fs = new FileStream(GetPath(path), FileMode.Append))
            {
                using (var fw = new StreamWriter(fs))
                {
                    fw.WriteLine(line);
                    fw.Flush();
                }
            }
        }

        /// <summary>
        /// Returns the compelte path for the file with the same name as provided in the string. The file is in the following format: "yyyy-MM-dd HH.mm.ss.fff - NAME.txt"
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetPath(string fileName)
        {
            // If this path has already been created in the current execution, then just returns that path
            if (_paths.Any(s => s.Contains(fileName)))
                return _paths.First(s => s.Contains(fileName));

            string path;
            lock (_lock)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RehaPanel\\Logs";

                // Creates the directory if this does not exists
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, _startTime.ToString("yyyy-MM-dd HH.mm.ss.fff") + " - " + fileName + ".txt");
            }
            
            // Adds the newly created path to the paths list
            _paths.Add(path);
            return path;
        }
    }
}