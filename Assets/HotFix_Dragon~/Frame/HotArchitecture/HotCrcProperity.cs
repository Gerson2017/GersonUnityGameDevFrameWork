

namespace HotGersonFrame
{
  public  class HotCrcProperity
    {


        public string Value;

        private uint s_curcrc;
        public  uint CurCrc
        {
            get
            {
                if (s_curcrc == 0)
                    s_curcrc = Crc32.GetCrc32(Value);
                return s_curcrc;
            }
        }


    }
}
