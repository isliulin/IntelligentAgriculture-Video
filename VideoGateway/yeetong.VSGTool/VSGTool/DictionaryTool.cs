using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VSGTool
{
    public static class DictionaryTool
    {
        static Dictionary<string, Thread> ThreadDictionary = new Dictionary<string, Thread>();

        public static bool Add(string key,Thread value)
        {
            try
            {
                if (ThreadDictionary.ContainsKey(key))
                {
                    Thread threadTemp = ThreadDictionary[key];
                    if (threadTemp != null && threadTemp.IsAlive)
                    {
                        threadTemp.Abort();
                        threadTemp = null;
                    }
                    ThreadDictionary[key] = value;
                }
                else
                    ThreadDictionary.Add(key, value);
                return true;
            }
            catch(Exception ){return false;}
        }

        public static bool Remove(string key)
        {
            try
            {
                if (ThreadDictionary.ContainsKey(key))
                {
                    Thread threadTemp = ThreadDictionary[key];
                    if (threadTemp != null && threadTemp.IsAlive)
                    {
                        threadTemp.Abort();
                        threadTemp = null;
                    }
                    ThreadDictionary.Remove(key);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool Clear()
        {
            try
            {
                foreach (var temp in ThreadDictionary)
                {
                    Thread threadTemp = temp.Value;
                    if (threadTemp != null && threadTemp.IsAlive)
                    {
                        threadTemp.Abort();
                        threadTemp = null;
                    }
                }
                ThreadDictionary.Clear();
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool IsExist(string key)
        {
            try
            {
                if (ThreadDictionary.ContainsKey(key))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { return false; }
        }


        //自检 把不存在的都删除了
        //keyList里面的每个项都被&所包
        public static bool SelfChecking(string keyList)
        {
            try
            {
                foreach (var temp in ThreadDictionary)
                {
                    string keyTemp = "&"+temp.Key+"&";
                    if (!keyList.Contains(keyTemp))
                    {
                        Thread threadTemp = ThreadDictionary[temp.Key];
                        if (threadTemp != null && threadTemp.IsAlive)
                        {
                            threadTemp.Abort();
                            threadTemp = null;
                        }
                        ThreadDictionary.Remove(temp.Key);
                    }
                }
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
