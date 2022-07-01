namespace GersonFrame
{
   public interface IStateController
    {
        void  InitState();
        bool ChangeAmState(string stateid,  object param1);

    }
}