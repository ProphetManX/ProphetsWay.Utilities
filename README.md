# ProphetsWay.Utilities
| Release   | Status |
|   ---     |  ---   |
| Latest Build: | [![Build status](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_apis/build/status/Utilities/Utilities%20CI)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_build/latest?definitionId=10)


| Alpha:    | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/3/3)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=3)
| Beta:     | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/3/10)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=3)
| Release:  | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/3/11)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=3)


A small library with a few useful utility classes.  They consist of basic functions that I end up using 
across many of my projects, so I put them here to share everywhere.

## Parser
This is the real hero of this project.  I use this class a lot in my web development because I end up working with
```string``` versions of my values and I want to get the actually typed value back out.

``` c#
    public static T GetValueFromKey<T>(this IDictionary<string, string> dictionary, string key)
    
    public static T GetValue<T>(this string input)
```

There are only two methods available, but really ```GetValue<T>``` is the one that does the work.  You can use
this right off any string variable/value and specify your type T, and it'll try to get your requested value type
out of that string.  The other method is a shortcut I created that works for string/string type dictionaries.  This simply gets the value 
out of the dictionary and fires ```GetValue<T>``` for you.

The following types are currently supported using this implementation.  *More can be added upon request.*

* DateTime
* DateTime?
* TimeSpan
* TimeSpan?
* short
* short?
* ushort
* ushort?
* int
* int?
* uint
* uint?
* long
* long?
* ulong
* ulong?
* double
* double?
* float
* float?
* bool
* bool?
* IPAddress

Just a quick example of how it works:

``` c#
    var idStr = "12345";
    var id = idStr.GetValue<int>();

    //or if you prefer
    var idLong = idStr.GetValue<long>();

    var ipStr = "192.168.0.1";
    var ip = ipStr.GetValue<IPAddress>();
```



## Streamer
This has two methods available, one will take a stream input and just return a byte array containing all the bytes
from that stream.  The other will take a byte array, and convert it into a ```MemoryStream``` so that you can
manipulate byte arrays with methods that require streams much easier.

As you can see below, there isn't much to these functions, you probably already have functions similar to this 
in projects that use streams.  This library as a whole isn't meant to be special, just a place to share all 
these small utility functions across multiple projects without having to write them over and over again.


``` c#
public static class Streamer
{
    private const int BufferSize = 1024 * 1024; //a megabyte buffer

    public static byte[] ReadToEnd(this Stream source)
    {
        var ms = new MemoryStream();
        var buffer = new byte[BufferSize];
        int bRead;

        do
        {
            bRead = source.Read(buffer, 0, buffer.Length);
            ms.Write(buffer, 0, bRead);
        } while (bRead > 0);

        return ms.ToArray();
    }

    public static Stream Streamify(this byte[] byteArr)
    {
        var ms = new MemoryStream(byteArr);
        return ms;
    }
}
```

## Serializer
This class consists of methods used to Serialize and Deserialize objects using built-in binary and Xml serialization
implementations.

### Binary Serializers
There are pairs of methods available, in the following section you can see there is a generic version of the method
to be used for most objects, however there is another pair that specifically take a ```string``` argument and return a 
```string``` value.  These are specifically implemented using ```UTF8``` to convert the string to bytes, and then
back.  The generic implementation uses the built-in ```BinaryFormatter```, due to this implementation, it is required 
that any custom objects you want to serialize, must be marked with the attribute ```[Serializable]```.

You will also notice that the methods are named "AsByteArr"/"FromByteArr" yet they take ```Stream``` as an argument 
or return one.  This is because in most cases I would be working with a file stream or some other sort of stream
that contains the object that I want to deserialize, or after I serialize the object, I will need to stream it somewhere.

If it's required to actually get the byte arrray values, then you can use ```Streamer``` to convert them back and fourth.

``` c#
    public static MemoryStream SerializeAsByteArr(this string stringToSerialize);

    public static string DeserializeFromByteArr(this Stream binaryStream);

    public static MemoryStream SerializeAsByteArr<T>(this T objectToSerialize);

    public static T DeserializeFromByteArr<T>(this Stream binaryStream);
```

In addition to the basic ser/deser methods above, I have also included two variants that will ser/deser directly into/from
a file.  These methods use the above functions to ser/deser and will additionally go ahead and write/read from your hard disk.

``` c#
    public static void SerializeAsByteArrToFile<T>(this T objectToSerialize, string targetFileName);

    public static T DeserializeFromByteArrInFile<T>(string targetFileName);
```

### Xml Serializers
Xml isn't going anywhere, and sometimes you just want to quickly serialize/deserialize without looking up how to do just that.
Below are the two methods you can use to interact with your objects and xml.

``` c#
    public static string SerializeAsXml<T>(this T objectToSerialize)

    public static T DeserializeFromXml<T>(this XmlDocument xmlDocument)
```

I personally struggeled with adding a third option method, one that would Deserialize and take a ```string``` argument.
I imagine as soon as I come across xml stored in a string I will add this method, however the current implementation
requires that you deserialize from an ```XmlDocument```.  You can convert your xml ```string``` easily by doing the following.

``` c#
var doc = new XmlDocument();
doc.LoadXml(xmlString);

var myObj = doc.DeserializeFromXml<MyClass>(); 
```

#### Something to note about Xml Serializers
There are some constructs that do not support Xml Serialization, if your object contains a ```Dictionary<TKey, TValue>```
then you might run into some problems using these methods even if you mark your class as ```[Serializable]```.  In 
these cases you can likely still use the Binary serializer options.


## Duplicator
This is a cheesy class, but I have come across a few places where I need to copy an object, and I don't want to maintain
the reference to the original (an actual duplicate object).  So this class simply Serializes and then Deserializes your 
object using the binary options above.

``` c#
public static T DuplicateObjectSerial<T>(this T original)
{
    var copy = original.SerializeAsByteArr().DeserializeFromByteArr<T>();

    return copy;
}
```

I have stubbed out (and commented it back out) another implementation for duplication that uses reflection, however it isn't
complete yet and I didn't want it to stop me from getting this library published, so I commented it out for now.  The 
current Duplicate method uses serialization, so you can only duplicate objects with the ```[Serializable]``` attribute.
The pending implementation won't have that requirement once it's fully finished.

