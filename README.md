# Accelbuffer - v2.1
`Accelbuffer` 是一个快速，高效的序列化系统。

# 更新日志

## 编译器
* 取消了对重复引用内置包的警告。
* Parser的分析结果不再使用Accelbuffer.Compiling.IDeclaration[]，改用Accelbuffer.Compiling.DeclarationArray。
* 部分自举，核心代码移至Accelbuffer程序集中（Accelbuffer.Compiling命名空间下），并支持在运行时编译代码（不推荐）。
* 优化了参数选项并暂时移除Java与C++参数，增加编译为AccelbufferByteCode（这个文件保存了所有元数据，在运行时拥有更快的解析速度）的途径。
* Declarations中描述类型名称的字段类型改用Accelbuffer.Compiling.TypeName，TypeName公开了Parser的分析结果，避免二次分析带来性能消耗。
* 支持运行时使用RuntimeCompiler编译并注入可序列化类型，并提供反射访问元数据（包括C#端和AccelbufferScript两部分元数据，Accelbuffer.Reflection命名空间下）。

## IL注入器
* 增加FacadeTypeAttribute提供注入序列化代理对外观类型的支持。
* 注入器代码重构（移至Accelbuffer.Injection.IL命名空间下，单独分离，并且公开）。
* SerializerInjector类型隐藏，原本公开的方法移至SerializerBinder（Accelbuffer.Injection命名空间下）。
* 将FieldIndexAttribute重命名为SerialIndexAttribute，使之与Accelbuffer.Reflection.AccelFieldInfo中提供的字段名称一致。

## 序列化&反序列化
* 提供更多扩展方法，从而更加便捷地序列化，反序列化。
* 在序列化基元类型时，如果值是默认值，不再忽略其写入操作。
* 支持使用System.Object作为ITypeSerializer<T>的泛型参数，但性能较差。
* Accelbuffer.ObjectType枚举的默认值不再使用Missing，而是有意义的Fixed0。
* Serializer类增加序列化事件回调注册，但无法为动态注入的可序列化类型注册事件。
* 提供UnsafeTypeSerializer实现Unmanaged类型以直接拷贝的方法快速序列化，但序列化数据无法跨平台。

## 内存管理器
* MemoryAllocator内部不再使用托管数组作为FreeList，改用固定大小的缓冲区。
* 为NativeMemory（可自动变长的非托管内存）提供一些扩展方法，读取与写入数据不需要再操作指针。
* 隐藏MemoryAllocator的Unsafe方法，推荐使用NativeMemory和NativeBuffer更加安全地操作非托管内存。

## 字符串编码器
* Encodings全部移至Accelbuffer.Unsafe.Text命名空间下。

## 类型
* System.IntPtr的关键字改为nint（Native Int）。
* System.UIntPtr的关键字改为nuint（Native Unsigned Int）。

## Unity3D
* 编辑器中提供更多编译选项。
* 移除所有Unity关键字，但仍然支持这些类型的内联序列化&反序列化。

## 其他
* 公开部分过去隐藏的类型与方法。
* MethodInfo，Type类型的字段全部改用RuntimeMethodHandle与RuntimeTypeHandle，避免这类大型对象长期闲置后仍无法被GC的问题。

# Others
* 作者联系方式 QQ：1024751595