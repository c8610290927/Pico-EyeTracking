using DataSync;



public class Test_FileTestData : LabDataBase
{
    public float TestFloat { get; set; }
    public int TestInt { get; set; }
    public string TestString { get; set; }

    public Test_FileTestData(float f, int i, string s)
    {
        TestFloat = f;
        TestInt = i;
        TestString = s;
    }

    public Test_FileTestData()
    {

    }

}
