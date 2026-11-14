namespace Ymck
{
    /// <summary>
    /// 泛型单例容器：首次访问 Instance 时创建 T 的实例。
    /// 注意：T 必须有公共无参构造函数；本实现不是线程安全的。
    /// </summary>
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                // 转成 object 再比较：T 未约束为 class，直接写 == null 无法编译
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}