

namespace HotGersonFrame
{
    public interface IHotCanGetModel : IHotBelongToArchitecture
    {

    }


    /// <summary>
    /// 使用静态扩展 赋予使用ICanGetModel 接口的对象获取到 IModel的接口方法 
    /// </summary>
    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this IHotCanGetModel self) where T :  class,IHotModel
        {
            return self.Architecture.GetModel<T>();
        }
    }

}
