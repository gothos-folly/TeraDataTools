using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DCTools.Structures;
using ProtoBuf;

namespace DCTools
{
    // ReSharper disable InconsistentNaming
    public class DCT
    // ReSharper restore InconsistentNaming
    {
        public static Dictionary<int, KeyValuePair<string, byte[]>> StringIds;
        public static Dictionary<int, KeyValuePair<string, byte[]>> ArgIds;
        public static string language = "eng";
        public static string txt = "";
        public static Dictionary<string, string> Strings;

        public static Dictionary<string, string> Args;

        public static List<KeyValuePair<string, string>> Values;

        public static List<KeyValuePair<Action, string>> Actions
            = new List<KeyValuePair<Action, string>>
                  {
                      new KeyValuePair<Action, string>(UnpackGer, "Unpack DataCenter (German)"),
                      new KeyValuePair<Action, string>(UnpackEng, "Unpack DataCenter (English)"),
                      new KeyValuePair<Action, string>(Test, "Test DataCenter"),
                      new KeyValuePair<Action, string>(Unique, "Get DataCenter unique values"),
                      new KeyValuePair<Action, string>(CleanUp, "Files CleanUp (.bin + .dec)"),
                  };

        public static Dictionary<string, Type> DcTypes
            = new Dictionary<string, Type>
                  {
                      {"01 00", typeof (int)},
                      {"02 00", typeof (float)},
                      {"05 00", typeof (bool)},
                      //Other is strings
                  };

        public static List<DcObject> DcObjects;

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n-----[ Tera DataCenter Tools ]-----");
                Console.WriteLine("(modified for rev. 3104 by CoolyT)\n");

                for (int i = 0; i < Actions.Count; i++)
                    Console.WriteLine("{0}: {1}", i + 1, Actions[i].Value);

                Console.WriteLine("OTHER: Exit");

                Console.Write("\nSelect action => ");

                int val;
              
                try
                {
                    val = int.Parse(Console.ReadLine() ?? "-1");
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                    break;
                }

                Actions[val - 1].Key();
            }
        }

        public static void CleanUp()
        {
            if (File.Exists("data.dec"))
                File.Delete("data.dec");
            
            if (File.Exists("structs.dec"))
                File.Delete("structs.dec");
            
            if (File.Exists("strings.dec"))
                File.Delete("strings.dec");
            
            if (File.Exists("strings_id.dec"))
                File.Delete("strings_id.dec");
            
            if (File.Exists("args_id.dec"))
                File.Delete("args_id.dec");
            
            if (File.Exists("args.dec"))
                File.Delete("args.dec");

            if (File.Exists("strings.bin"))
                File.Delete("strings.bin");

            if (File.Exists("strings_id.bin"))
                File.Delete("strings_id.bin");

            if (File.Exists("args_id.bin"))
                File.Delete("args_id.bin");

            if (File.Exists("dc.bin"))
                File.Delete("dc.bin");
        }
        public static void CutFilesGer()
        {            
                CutFileData("DataCenter_Final_GER.bin", "data.dec", 0x00000340, 0x03FF8B10);           
                CutFileData("DataCenter_Final_GER.bin", "structs.dec", 0x04000754, 0x069E101C); 
                CutFileData("DataCenter_Final_GER.bin", "strings.dec", 0x06A00898, 0x0A7E5CD0); 
                CutFileData("DataCenter_Final_GER.bin", "strings_id.dec", 0x0B48E0B4, 0x0B7B0EDC); 
                CutFileData("DataCenter_Final_GER.bin", "args_id.dec", 0xB80D9A4, 0xB814A50);
                CutFileData("DataCenter_Final_GER.bin", "args.dec", 0x0B7B0EE8, 0x0B7E0442);
/*
                if (!File.Exists("data.dec"))
                    CutFileData("dc_new.dec", "data.dec", 0x00000340, 0x03FF8B10); //0x00000294, 0x025D0B04

                if (!File.Exists("structs.dec"))
                    CutFileData("dc_new.dec", "structs.dec", 0x04000754, 0x069E1028); //0x02600508, 0x039F5100           

                //            if (!File.Exists("structs.dec"))
                //                CutFileData("dc_old.dec", "structs.dec", 0x02600508, 0x039F5100); //

                if (!File.Exists("strings.dec"))
                    CutFileData("dc_new.dec", "strings.dec", 0x06A00898, 0x0A7E5CD0); //0x03A0059C, 0x05320444

                if (!File.Exists("strings_id.dec"))
                    CutFileData("dc_new.dec", "strings_id.dec", 0x0B48E0B4, 0x0B7B0EDC); //0x058B7910, 0x05A1D05C

                if (!File.Exists("args.dec"))
                    CutFileData("dc_new.dec", "args.dec", 0x0B7B0EE8, 0x0B7E0442);  //0x05A1D068, 0x05A382DC
*/
        }

        public static void CutFilesEng()
        {
                CutFileData("DataCenter_Final_EUR.bin", "data.dec", 0x00000340, 0x04000734);
                CutFileData("DataCenter_Final_EUR.bin", "structs.dec", 0x04000754, 0x06A0088C);
                CutFileData("DataCenter_Final_EUR.bin", "strings.dec", 0x06A00898, 0x0A61D8F2);
                CutFileData("DataCenter_Final_EUR.bin", "strings_id.dec", 0x0B2A79AC, 0x0B5C8E30);
                CutFileData("DataCenter_Final_EUR.bin", "args_id.dec", 0xB6258D8, 0xB62C97C);
                CutFileData("DataCenter_Final_EUR.bin", "args.dec", 0x0B5C8E3C, 0x0B5F8372);
        }
        
        public static void UnpackGer()
        {
            Unpack("ger");
        }

        public static void UnpackEng()
        {
            Unpack("eng");
        }

        public static void Unpack(string lang)
        {
            language = lang;

            Console.WriteLine("\n--- DC Unpack ---\n");
            Stopwatch stopwatch = Stopwatch.StartNew();
            

            //Devide DataCenter
            switch (language)
            {
                case "eng":
                {
                    CutFilesEng();
                    break;
                }

                case "ger":
                {
                    CutFilesGer();
                    break;
                }
            }

            #region StringIds
            Console.Write("processing String IDs: ");
            
            if (!File.Exists("strings_id.bin"))
            {
                ReadStringIds("strings_id.dec");

                using (FileStream fs = File.Create("strings_id.bin"))
                {
                    Serializer.Serialize(fs, StringIds);
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead("strings_id.bin"))
                {
                    StringIds = Serializer.Deserialize<Dictionary<int, KeyValuePair<string, byte[]>>>(fs);
                }
            }

            Console.WriteLine("{0} - DONE", StringIds.Count);

            #endregion

            #region Strings

            Console.Write("processing Strings   : ");

            if (!File.Exists("strings.bin"))
            {
                ReadStrings("strings.dec", 0x020000);

                using (FileStream fs = File.Create("strings.bin"))
                {
                    Serializer.Serialize(fs, Strings);
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead("strings.bin"))
                {
                    Strings = Serializer.Deserialize<Dictionary<string, string>>(fs);
                }
            }

            Console.WriteLine("{0} - DONE", Strings.Count);

            #endregion
                        
            Console.Write("processing Args IDs  : ");

            if (!File.Exists("args_id.bin"))
            {
                ReadArgIds("args_id.dec");

                using (FileStream fs = File.Create("args_id.bin"))
                {
                    Serializer.Serialize(fs, ArgIds);
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead("args_id.bin"))
                {
                    ArgIds = Serializer.Deserialize<Dictionary<int, KeyValuePair<string, byte[]>>>(fs);
                }
            }

            Console.WriteLine("{0} - DONE", ArgIds.Count+1);


            ReadArgs("args.dec");

            ReadValues("data.dec");

            ReadObjects("structs.dec");

            Console.WriteLine("--------------------------------------------------");

            DataCenter dataCenter = new DataCenter
                                        {
                                            Values = Values,
                                            Objects = DcObjects,
                                            MainObjects = new List<DcObject>(),
                                        };

            bool[] used = new bool[DcObjects.Count];
            int counter = 1;
            //go trough all Objects
            for (int i = 0; i < DcObjects.Count; i++)

                for (int j = 0; j < DcObjects[i].SubCount; j++)
                    used[DcObjects[i].SubShift + j] = true;

            for (int i = 0; i < DcObjects.Count; i++)
            {
                txt = "\rObject -> MainObject : " + counter+1;
                Console.Write(txt + " - {0}%", (100f * i / DcObjects.Count).ToString("0.00"));
                if (used[i] || DcObjects[i].Name == "Hash")
                 continue;

               dataCenter.MainObjects.Add(DcObjects[i]);
               counter++;
            }

            Console.Out.WriteLine(txt+ " - DONE            ");
            Console.Write("building DataCenter protobuf...");
            using (FileStream fs = File.Create("dc.bin"))
            {
                Serializer.Serialize(fs, dataCenter);
            }
            Console.WriteLine(" - DONE");
            Console.WriteLine("--------------------------------------------------");
            stopwatch.Stop();
            Console.WriteLine("\rAll done in {0}s", (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.00"));
            Console.WriteLine("--------------------------------------------------");
        }

        public static void Test()
        {
            Console.WriteLine("\n--- DC Test ---\n");

            DataCenter dc = GetDataCenter();

            var objects = dc.GetObjectsByName("SkillData");
            foreach (var dcObject in objects)
            {
                var values = dc.GetValues(dcObject);
                Console.WriteLine(values["huntingZoneId"]);
            }
        }

        public static void Unique()
        {
            Console.WriteLine("\n--- DC Unique ---\n");

            DataCenter dc = GetDataCenter();

            Console.Write("Write object name => ");
            string objectName = Console.ReadLine();

            Console.Write("Write field name => ");
            string fieldName = Console.ReadLine();

            Console.WriteLine();

            List<string> uniqueValues = new List<string>();

            var objects = dc.GetObjectsByName(objectName);

            foreach (var dcObject in objects)
            {
                var values = dc.GetValues(dcObject);

                if (values.ContainsKey(fieldName))
                {
                    if (uniqueValues.Contains(values[fieldName].ToString()))
                        continue;

                    Console.WriteLine(values[fieldName]);
                    uniqueValues.Add(values[fieldName].ToString());
                }
            }

            using (TextWriter w = new StreamWriter(objectName + "_" + fieldName + ".txt"))
            {
                for (int i = 0; i < uniqueValues.Count; i++)
                    w.WriteLine("{0},", uniqueValues[i]);
            }

            Process.Start(objectName + "_" + fieldName + ".txt");
        }

        private static DataCenter _dataCenter;

        public static DataCenter GetDataCenter(string path = "dc.bin")
        {
            if (_dataCenter == null)
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    _dataCenter = Serializer.Deserialize<DataCenter>(fs);
                }
            }

            return _dataCenter;
        }

        static void ReadStringIds(string path)
        {
            StringIds = new Dictionary<int, KeyValuePair<string, byte[]>>();
            int counter = 0;

            using (FileStream fin = File.OpenRead(path))
            {
                while (fin.Position < fin.Length)
                {
                    byte[] data = new byte[4];
                    int readed = fin.Read(data, 0, data.Length);

                    string hex = BitConverter.ToString(data, 0, readed).Replace("-", " ");

                     StringIds.Add(counter++, new KeyValuePair<string, byte[]>(hex, data));
                }
            }
        }

        static void ReadArgIds(string path)
        {
            ArgIds = new Dictionary<int, KeyValuePair<string, byte[]>>();
            int counter = 0;

            using (FileStream fin = File.OpenRead(path))
            {
                while (fin.Position < fin.Length)
                {
                    byte[] data = new byte[4];
                    int readed = fin.Read(data, 0, data.Length);

                    string hex = BitConverter.ToString(data, 0, readed).Replace("-", " ");

                    ArgIds.Add(counter++, new KeyValuePair<string, byte[]>(hex, data));
                }
            }
        }

        static void ReadStrings(string path, long del)
        {
            Strings = new Dictionary<string, string>();
            Encoding encoding = Encoding.Unicode;

            long realLength = 0;

            using (FileStream fin = File.OpenRead(path))
            {
                int counter = 0;
                using (BinaryReader r = new BinaryReader(fin)) // Strings
                {
                   
                    foreach (var val in StringIds.Values)
                    {
                        byte[] shiftData = new byte[4];
                        shiftData[0] = val.Value[2];
                        shiftData[1] = val.Value[3];
                        shiftData[2] = val.Value[0];
                        shiftData[3] = val.Value[1];

                        long shift = (BitConverter.ToUInt32(shiftData, 0)*2);
                        var wtf = (int)(shift / del) * 8;
                        shift += wtf;

                        fin.Seek(shift, SeekOrigin.Begin);

                        string s = "";
                        short ch;
                        int c = counter;
                        while ((ch = r.ReadInt16()) != 0)
                            s += encoding.GetString(BitConverter.GetBytes(ch));

                        if (Strings.ContainsKey(val.Key))
                        {
                            Console.Out.WriteLine("ERROR: Duplicate StringID : "+val.Key+" Pos: "+(shift -s.Length));
                            continue;
                        }
                        Strings.Add(val.Key, s);

                        if (fin.Position > realLength)
                            realLength = fin.Position;
                        counter++;
                    }
                }
                List<string> log = new List<string>();
                foreach (var str in Strings)
                {
                    log.Add("ID: " + str.Key + " - " + str.Value);
                }

                File.WriteAllLines(Path.GetFullPath("./") + "strings.txt", log);
            }
        }

        static void ReadArgs(string path)
        {
            Args = new Dictionary<string, string> { { "00 00", "Hash" } };
            Encoding encoding = Encoding.Unicode;
            long spacer = 0x20000;

            long realLength = 0;

            using (FileStream fin = File.OpenRead(path))
            {
                int counter = 0;
                using (BinaryReader r = new BinaryReader(fin)) // Strings
                {

                    foreach (var val in ArgIds.Values)
                    {
                        byte[] shiftData = new byte[4];
                        shiftData[0] = val.Value[2];
                        shiftData[1] = val.Value[3];
                        shiftData[2] = val.Value[0];
                        shiftData[3] = val.Value[1];

                        long shift = (BitConverter.ToUInt32(shiftData, 0) * 2);
                        var wtf = (int)(shift / spacer) * 8;
                        shift += wtf;

                        fin.Seek(shift, SeekOrigin.Begin);

                        string s = "";
                        short ch;
                        int c = counter;
                        while ((ch = r.ReadInt16()) != 0)
                            s += encoding.GetString(BitConverter.GetBytes(ch));

                        Args.Add(BitConverter.ToString(BitConverter.GetBytes(Args.Count), 0, 2).Replace("-", " "), s);

                        txt = "\rprocessing Args      : "+Args.Count;
                        Console.Write(txt + " - {0}%", (100f * Args.Count / (ArgIds.Count+1)).ToString("0.00"));                        
                        if (fin.Position > realLength)
                            realLength = fin.Position;
                        counter++;
                    }
                }
                
                //only for Debug
                List<string> log = new List<string>();
                foreach (var str in Args)
                {
                    log.Add("ID: " + str.Key + " - " + str.Value);
                }

                File.WriteAllLines(Path.GetFullPath("./") + "args.txt", log);
            }
            Console.WriteLine(txt+ " - DONE           ");
        }

        static void ReadValues(string path)
        {
            Values = new List<KeyValuePair<string, string>>();
           
            using (FileStream decStream = File.OpenRead(path))
            {
                byte[] buffer = new byte[0x80000]; //Do not change

                bool argFound = false;
                Type type = null;

                string key = "", typ = "";

                while (decStream.Position < decStream.Length)
                {
                    int readed = decStream.Read(buffer, 0, buffer.Length);

                    for (int i = 0; i < readed; i += 4)
                    {
                        if (argFound)
                        {
                            object val = null;

                            if (type == typeof (int))
                                val = BitConverter.ToInt32(buffer, i);
                            else if (type == typeof (uint))
                                val = BitConverter.ToUInt32(buffer, i);
                            else if (type == typeof (float))
                                val = BitConverter.ToSingle(buffer, i);
                            else if (type == typeof (bool))
                                val = buffer[i] > 0;
                            else
                            {
                                string hex = BitConverter.ToString(buffer, i, 4).Replace("-", " ");

                                if (Strings.ContainsKey(hex))
                                    val = Strings[hex];
                                else
                                    Console.WriteLine("UNKNOWN DATA: {0} {1}", typ, hex);
                            }

                            argFound = false;
                            type = null;

                            Values.Add(new KeyValuePair<string, string>(key, "" + val));
                        }
                        else
                        {
                            string hex = BitConverter.ToString(buffer, i, 4).Replace("-", " ");

                            key = hex.Substring(0, 5);
                            typ = hex.Substring(6);

                            if (DcTypes.ContainsKey(typ))
                                type = DcTypes[typ];

                            key = Args[key];
                            argFound = true;
                        }
                    }

                    decStream.Read(buffer, 0, 8); //Spacer
                          
                    txt = "\rprocessing Data      : " + Values.Count;
                    Console.Write(txt + " - {0}%", (100f*decStream.Position/decStream.Length).ToString("0.00"));
                }

                Console.WriteLine(txt + " - DONE              ");
            }
        }

        static void ReadObjects(string path)
        {
            int counter = 1; ;
            DcObjects = new List<DcObject>();
            List<string> log = new List<string>();

            using (FileStream decStream = File.OpenRead(path))
            {
                //first Spacer
                long del = 0xFFFF0;
                
                while (decStream.Position < decStream.Length)
                {
                    byte[] data = new byte[16];

                    if (decStream.Position == del)
                    {
                        decStream.Read(data, 0, 8);
                        //every (65536 objects (+ 1 nullvalue object (x 16 bytes) = ) 1048576 bytes is the next spacer located
                        del += 0x100008;
                    }

                    decStream.Read(data, 0, data.Length);

                    string keyHex = BitConverter.ToString(data, 0, 2).Replace("-", " ");

                    string name = Args[keyHex];
                    int argsCount = BitConverter.ToUInt16(data, 4);
                    int subCount = BitConverter.ToUInt16(data, 6);

                    // Data Structure Example : D6 00, 00 00, 04 00, 00 00, 00 00 0C 00, FF FF FF FF
                    // Arg (D6 00),nothing (00 00),argsCount (04 00) subCount (00 00), argsShift (00 00 0C 00), subShift (FF FF FF FF)

                    //just invert the byteStreamData to get out a valid int
                    byte[] shiftData = new byte[4];
                    shiftData[0] = data[10];
                    shiftData[1] = data[11];
                    shiftData[2] = data[8];
                    shiftData[3] = data[9];

                    int argsShift = BitConverter.ToInt32(shiftData, 0);

                    shiftData[0] = data[14];
                    shiftData[1] = data[15];
                    shiftData[2] = data[12];
                    shiftData[3] = data[13];

                    int subShift = BitConverter.ToInt32(shiftData, 0) - 1;

                    DcObject dcObject = new DcObject
                                            {
                                                Name = name,

                                                ArgsCount = argsCount,
                                                ArgsShift = argsShift,

                                                SubCount = subCount,
                                                SubShift = subShift,
                                            };

                    DcObjects.Add(dcObject);
                    txt = "\rprocessing Objects   : " + DcObjects.Count;
                    Console.Write(txt + " - {0}%", (100f*decStream.Position/decStream.Length).ToString("0.00"));

                
                    counter++;
                }
                Console.WriteLine(txt + " - DONE              ");
            }
        }

        static void CutFileData(string fromPath, string toPath, long from, long to)
        {
            using (FileStream fin = File.OpenRead(fromPath))
            {
                using (FileStream fout = File.Create(toPath))
                {
                    fin.Seek(from, SeekOrigin.Begin);

                    while (fin.Position < to)
                    {
                        long size = to - fin.Position;

                        if (size > short.MaxValue)
                            size = short.MaxValue;

                        byte[] data = new byte[size];
                        int readed = fin.Read(data, 0, data.Length);

                        if (readed == 0)
                            break;

                        fout.Write(data, 0, readed);
                    }
                }
            }
        }
    }
}
