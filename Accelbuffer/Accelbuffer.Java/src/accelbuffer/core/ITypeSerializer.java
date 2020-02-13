package accelbuffer.core;

/**
 * 实现接口完成指定类型对象的序列化和反序列化
 * @param <T> 序列化的对象的类型
 */
public interface ITypeSerializer<T> {
    /**
     * 方法用于实现对象的序列化
     * @param obj 将被序列化的对象
     * @param writer 数据输出流
     */
    public void serialize(T obj, AccelWriter writer);

    /**
     * 方法用于实现对象的反序列化
     * @param reader 数据输入流
     * @return 反序列化对象
     */
    public T deserialize(AccelReader reader);
}
