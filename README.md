# Accelbuffer - v1.4
`Accelbuffer` 是一个快速，高效的序列化系统。

## 支持
* 需要.NET Framework 4.5+
* 提供unity插件
* 提供unity生成配置
* 提供sublime的语法高亮文件

## 特点
* 时间消耗低
* 托管堆内存分配接近于对象的真实大小
* 无装箱、拆箱
* 可以自定义序列化流程
* 没有序列化深度限制

## 支持的序列化类型
* 基元类型
* 一维数组，交错数组
* `List<T>`, `IList<T>`
* `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`
* `ISerializableEnumerable<T>`
* `ICollection<T>`
* `Type`
* `class`, `struct`

> Unity配置下，新增类型
* `Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`, `Quaternion`

## 基本用法
### 1.使用特性标记类型
#### 方案一，自动注入代理
```csharp
[Serializable]
public class Student
{
  [FieldIndex(1)]
  public int Id;

  [FieldIndex(2)]
  [Encoding(Encoding.Unicode)]
  public string Name;

  [CheckRef]
  [FieldIndex(3)]
  public List<string> FriendNames;

  public Student() { }
}
```

#### 方案二，手动实现代理
```csharp
[SerializeBy(typeof(StudentSerializer))]
public class Student
{
  public int Id;
  public string Name;
  public List<string> FriendNames;

  public Student() { }
}

internal sealed class StudentSerializer : ITypeSerializer<Student>
{
  public void Serialize(Student obj, ref StreamingWriter writer)
  {
    writer.WriteValue(1, obj.Id, NumberFormat.Variant);
    writer.WriteValue(2, obj.Name, Encoding.Unicode);
  
    if (obj.FriendNames != null)
    {
      writer.WriteValue<List<string>>(3, obj.FriendNames);
    }
  }
  public Student Deserialize(ref StreamingIterator iterator)
  {
    Student result = new Student();
    int index = 0;

    while(iterator.HasNext(out index))
    {
      switch(index)
      {
        case 1:
          result.Id = iterator.NextAsInt32();
          break;
        case 2:
          result.Name = iterator.NextAsString();
          break;
        case 3:
          result.FriendNames = iterator.NextAs<List<string>>();
          break;
        default:
          iterator.SkipNext();
          break;
      }
    }
    return result;
  }
}
```

#### 方案三，使用`Accelbuffer`语言
```csharp
using System.Collections.Generic;

ref type Student
{
  int Id;
  unicode string Name;
  checkref List<string> FriendNames;
}
```

* 命令行中输入
> accelc -c C:\Users\Administrator\Desktop\test.accel

* 如果输出以下内容，则编译成功
> 文件生成成功
> 文件路径：C:\Users\Administrator\Desktop\test.accel.cs

### 2.序列化对象
```csharp
Student student = new Student { ... };
byte[] data = Serializer.Serialize<Student>(student);
```

### 3.反序列化对象
```csharp
byte[] data = File.ReadAllBytes(someFile);
Student student = Serializer.Deserialize<Student>(data, 0, data.Length);
```

### 4.释放序列化缓冲区内存
> 如果内存正在被使用，会导致释放失败，但不会抛出异常
> 如果一个类型经常被序列化，可以选择不释放内存，以减少内存的频繁分配

##### 方法一、 释放指定一个类型的缓冲区内存
```csharp
Serializer.GetAllocator<Student>().TryFreeMemory();
```

##### 方法二、 释放所有序列化器分配的缓冲区内存
```csharp
MemoryAllocator.FreeAllAvailableMemory();
```

### 5.序列化事件回调
> 这个方法不会造成值类型的装箱，且可以存在多个回调方法，这些方法必须是公共的实例方法且可以通过`Priority`指定调用顺序

```csharp
[Serializable]
public class Student
{
  [FieldIndex(1)]
  public int Id;
  [FieldIndex(2)]
  [Encoding(Encoding.Unicode)]
  public string Name;
  [CheckRef]
  [FieldIndex(3)]
  public List<string> FriendNames;

  [OnBeforeSerialization(Priority = 0)]
  public void Before()
  {
    Console.WriteLine("Before Serialization");
  }

  [OnAfterDeserialization(Priority = 0)]
  public void After()
  {
    Console.WriteLine("After Deserialization");
  }

  public Student() { }
}
```

> 建议使用`Accelbuffer`语言，以下是等价的写法

```csharp
using System;
using System.Collections.Generic;

ref runtime type Student
{
  int Id;
  unicode string Name;
  checkref List<string> FriendNames;

  .before
  {
    Console.WriteLine("Before Serialization");
  }

  .after
  {
    Console.WriteLine("After Deserialization");
  }
}
```

* 作者联系方式 QQ：1024751595