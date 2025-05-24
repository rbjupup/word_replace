
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

        //�ַ����������滻��ʱ���������汾
        public LString(string key, string defaultKey, params object[] param)
        {
            LKey = key;
            LDefaultValue = defaultKey;
            LStringParam = param;
        }

        public string LKey;
        public string LDefaultValue;
        public object[] LStringParam;


        //LString �� String����ʽת��
        public static implicit operator string(LString lstr)
        {
            return lstr.ToString();
        }


        //Todo ����Ҫд�ɶ�ȡ���ݿ�,Ȼ��������ݿ��ȡ�����ַ������з���
        public override string ToString()
        {
            return LDefaultValue;
        }
    }

}
