namespace HotGersonFrame
{
    public interface IHotStateController
    {
        void InitState();
        bool ChangeAmState(string stateid, object param1=null, object param2=null,object param3=null);

    }
}