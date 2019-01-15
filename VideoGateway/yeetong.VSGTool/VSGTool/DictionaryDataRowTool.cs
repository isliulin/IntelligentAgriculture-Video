using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VSGTool
{
    public static class DictionaryDataRowTool
    {
        static Dictionary<string, DataRow> DataRowDictionary = new Dictionary<string, DataRow>();

        public static bool Add(string key,DataRow value)
        {
            try
            {
                if (DataRowDictionary.ContainsKey(key))
                {
                    DataRowDictionary[key] = value;
                }
                else
                    DataRowDictionary.Add(key, value);
                return true;
            }
            catch(Exception ){return false;}
        }

        public static bool Remove(string key)
        {
            try
            {
                if (DataRowDictionary.ContainsKey(key))
                {
                    DataRowDictionary.Remove(key);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public static DataRow GetValueByKey(string key)
        {
            try
            {
                if (DataRowDictionary.ContainsKey(key))
                {
                   return  DataRowDictionary[key];
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        public static bool Clear()
        {
            try
            {
                DataRowDictionary.Clear();
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool IsExist(string key)
        {
            try
            {
                if (DataRowDictionary.ContainsKey(key))
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
                foreach (var temp in DataRowDictionary)
                {
                    string keyTemp = "&"+temp.Key+"&";
                    if (!keyList.Contains(keyTemp))
                    {
                        DataRowDictionary.Remove(temp.Key);
                    }
                }
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
