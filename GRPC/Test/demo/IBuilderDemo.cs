using Reformat.Framework.Core.GRPC.Attributes;

namespace Reformat.Framework.Core.GRPC.Test.demo;

[ProtoSource("proto", "StudentService","WebApi")]
public interface IBuilderDemo
{
    Student GetStudent(People people);
    Student GetStudent2(People ID);
}

public class People
{
    public string name { get; set; }
}

public class Student
{
    public long id { get; set; }
    public string name { get; set; }
    public int age { get; set; }
}