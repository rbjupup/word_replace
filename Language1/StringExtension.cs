
using System.Windows.Input;

namespace Language
{
    public class LString
    {
        public LString(string key, params object[] param)
        {
            LKey = key;
            LStringParam = param;
            LDefaultValue = key;
        }

        //字符串带参数替换的时候调用这个版本
        public LString(string key, string defaultKey, params object[] param)
        {
            LKey = key;
            LDefaultValue = defaultKey;
            LStringParam = param;
        }

        public string LKey;
        public string LDefaultValue;
        public object[] LStringParam;


        //LString 到 String的隐式转化
        public static implicit operator string(LString lstr)
        {
            return lstr.ToString();
        }


        //Todo 这里要写成读取数据库,然后根据数据库读取到的字符串进行返回
        public override string ToString()
        {
            return LDefaultValue;
        }
    }

}
