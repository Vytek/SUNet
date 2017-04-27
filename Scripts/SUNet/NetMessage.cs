using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NetMessage {

    private byte[] buff;
    private int index = 0;

    //--------------------------------------------------------------------------------------------------------
    public NetMessage() {
        ResetBuffer();
    }
    public NetMessage(byte[] arr, bool deepCopy=false) {
        if (deepCopy) {
            LoadDeep(arr);
        }
        else {
            LoadShallow(arr);
        }
    }
    public NetMessage(byte[] arr, int startIndexAt, bool deepCopy=false) {
        if (deepCopy) {
            LoadDeep(arr);
        }
        else {
            LoadShallow(arr);
        }
        index = startIndexAt;
    }
    public NetMessage(byte[] arr, int copyLength, int startIndexAt) {
        LoadDeep(arr, copyLength);
        index = startIndexAt;
    }
    public NetMessage(NetMessage source, int startFrom = 0) {
            buff = new byte[source.GetLength() - startFrom];
            Buffer.BlockCopy(source.ToArray(), startFrom, buff, 0, source.GetLength()-startFrom);
    }

    //--------------------------------------------------------------------------------------------------------
    public void ResetIndex() { 
        index = 0;
    }

    //--------------------------------------------------------------------------------------------------------
    public void ResetBuffer() {
        buff = new byte[0];
        index = 0;
    }

    //--------------------------------------------------------------------------------------------------------
    public int GetLength() {
        return buff.Length;
    }

    //--------------------------------------------------------------------------------------------------------
    public byte[] ToArray() {
        return buff;
    }

    //--------------------------------------------------------------------------------------------------------
    public void LoadDeep(byte[] data, int length=-1) {
        if (length == -1) { length = data.Length; }
        ResetBuffer();
        if (length == 0) return;
        buff = new byte[length];
        Buffer.BlockCopy(data, 0, buff, 0, length);
    }

    //--------------------------------------------------------------------------------------------------------
    public void LoadShallow(byte[] data) {
        ResetBuffer();
        buff = data;
    }

    //--------------------------------------------------------------------------------------------------------
    public byte[] Encode(params object[] args) {
        if (args.Length > 255) {
            throw new Exception("Encoding Error: Max Number of arguments is 255!");
        }
   
        ResetBuffer();
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == null) {
                throw new Exception("Encoding Error: Invalid argument value -> NULL");
            }
            EncodeObj(args[i]);
        }
        return buff;
    } //Encode
    private void EncodeObj(object obj) {
        if (obj == null) { return; }
        if (obj is bool) { Push((bool)obj); }
        else if (obj is byte) { Push((byte)obj); }
        else if (obj is sbyte) { Push((sbyte)obj); }
        else if (obj is Int16) { Push((Int16)obj); }
        else if (obj is Int32) { Push((Int32)obj); }
        else if (obj is Int64) { Push((Int64)obj); }
        else if (obj is UInt16) { Push((UInt16)obj); }
        else if (obj is UInt32) { Push((UInt32)obj); }
        else if (obj is UInt64) { Push((UInt64)obj); }
        else if (obj is float) { Push((float)obj); }
        else if (obj is double) { Push((double)obj); }
        else if (obj is char) { Push((char)obj); }
        else if (obj is string) { Push((string)obj); }
        else if (obj is DateTime) { Push((DateTime)obj); }
        else if (obj is Vector2) { Push((Vector2)obj); }
        else if (obj is Vector3) { Push((Vector3)obj); }
        else if (obj is Quaternion) { Push((Quaternion)obj); }
        else if (obj is bool[]) { Push((bool[])obj); }
        else if (obj is byte[]) { Push((byte[])obj); }
        else if (obj is sbyte[]) { Push((sbyte[])obj); }
        else if (obj is Int16[]) { Push((Int16[])obj); }
        else if (obj is Int32[]) { Push((Int32[])obj); }
        else if (obj is Int64[]) { Push((Int64[])obj); }
        else if (obj is UInt16[]) { Push((UInt16[])obj); }
        else if (obj is UInt32[]) { Push((UInt32[])obj); }
        else if (obj is UInt64[]) { Push((UInt64[])obj); }
        else if (obj is float[]) { Push((float[])obj); }
        else if (obj is double[]) { Push((double[])obj); }
        else if (obj is char[]) { Push((char[])obj); }
        else if (obj is string[]) { Push((string[])obj); }
        else if (obj is Vector2[]) { Push((Vector2[])obj); }
        else if (obj is Vector3[]) { Push((Vector3[])obj); }
        else if (obj is Quaternion[]) { Push((Quaternion[])obj); }
        else if (obj is DateTime[]) { Push((DateTime[])obj); }
        else if (obj is List<bool>) { Push((List<bool>)obj); }
        else if (obj is List<byte>) { Push((List<byte>)obj); }
        else if (obj is List<sbyte>) { Push((List<sbyte>)obj); }
        else if (obj is List<Int16>) { Push((List<Int16>)obj); }
        else if (obj is List<Int32>) { Push((List<Int32>)obj); }
        else if (obj is List<Int64>) { Push((List<Int64>)obj); }
        else if (obj is List<UInt16>) { Push((List<UInt16>)obj); }
        else if (obj is List<UInt32>) { Push((List<UInt32>)obj); }
        else if (obj is List<UInt64>) { Push((List<UInt64>)obj); }
        else if (obj is List<float>) { Push((List<float>)obj); }
        else if (obj is List<double>) { Push((List<double>)obj); }
        else if (obj is List<char>) { Push((List<char>)obj); }
        else if (obj is List<string>) { Push((List<string>)obj); }
        else if (obj is List<DateTime>) { Push((List<DateTime>)obj); }
        else if (obj is List<Vector2>) { Push((List<Vector2>)obj); }
        else if (obj is List<Vector3>) { Push((List<Vector3>)obj); }
        else if (obj is List<Quaternion>) { Push((List<Quaternion>)obj); }
        else { PushSerializable(obj); }
    } //EncodeObj

    //--------------------------------------------------------------------------------------------------------
    public void Push(bool val) { buff = ConcatByteArrays(buff, new byte[] { (byte) (val==true?1:0) });  }
    public void Push(byte val) { buff = ConcatByteArrays(buff, new byte[] { val }); }
    public void Push(sbyte val) { buff = ConcatByteArrays(buff, new byte[] { (byte)val }); }
    public void Push(Int16 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(Int32 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(Int64 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(UInt16 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(UInt32 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(UInt64 val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(float val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(double val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(char val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val)); }
    public void Push(DateTime val) { buff = ConcatByteArrays(buff, BitConverter.GetBytes(val.Ticks)); }
    public void Push(string val) {
        byte[] barr = Encoding.Unicode.GetBytes(val);
        buff = ConcatByteArrays(buff, BitConverter.GetBytes(barr.Length), barr);
    }
    public void PushB64(string val) {
        byte[] barr = Encoding.UTF8.GetBytes(val);
        string b64 = Convert.ToBase64String(barr);
        barr = Encoding.Unicode.GetBytes(b64);
        buff = ConcatByteArrays(buff, BitConverter.GetBytes(barr.Length), barr);
    }
    public void PushB64(string[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            PushB64(arr[i]);
        }
    }
    public void Push(Vector2 v2) {
        Push(v2.x);
        Push(v2.y);
    }
    public void Push(Vector3 v3) {
        Push(v3.x);
        Push(v3.y);
        Push(v3.z);
    }
    public void Push(Quaternion quat) {
        Push(quat.x);
        Push(quat.y);
        Push(quat.z);
        Push(quat.w);
    }
    public void PushSerializable(object obj) {
        if (obj == null) {
            return;
        }
        if (!obj.GetType().IsSerializable) {
            throw new Exception("Object [" + obj.GetType().Name + "] should be serializable!");
        }
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        byte[] barr = ms.ToArray();
        buff = ConcatByteArrays(buff, BitConverter.GetBytes(barr.Length), barr);
    } //PushSerializable

    //--------------------------------------------------------------------------------------------------------
    public void PushFront(bool val) { buff = ConcatByteArrays(new byte[] { (byte)(val == true ? 1 : 0) }, buff); }
    public void PushFront(byte val) { buff = ConcatByteArrays(new byte[] { val }, buff); }
    public void PushFront(sbyte val) { buff = ConcatByteArrays(new byte[] { (byte)val }, buff); }
    public void PushFront(Int16 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(Int32 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(Int64 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(UInt16 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(UInt32 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(UInt64 val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(float val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(double val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(char val) { buff = ConcatByteArrays(BitConverter.GetBytes(val), buff); }
    public void PushFront(DateTime val) { buff = ConcatByteArrays(BitConverter.GetBytes(val.Ticks), buff); }
    public void PushFront(string val) {
        byte[] barr = Encoding.Unicode.GetBytes(val);
        buff = ConcatByteArrays(BitConverter.GetBytes(barr.Length), barr, buff);
    }
    public void PushFrontB64(string val) {
        byte[] barr = Encoding.UTF8.GetBytes(val);
        string b64 = Convert.ToBase64String(barr);
        barr = Encoding.Unicode.GetBytes(b64);
        buff = ConcatByteArrays(BitConverter.GetBytes(barr.Length), barr, buff);
    }
    public void PushFront(Vector2 v2) {
        PushFront(v2.y);
        PushFront(v2.x);
    }
    public void PushFront(Vector3 v3) {
        PushFront(v3.z);
        PushFront(v3.y);
        PushFront(v3.x);
    }
    public void PushFront(Quaternion quat) {
        PushFront(quat.w);
        PushFront(quat.z);
        PushFront(quat.y);
        PushFront(quat.x);
    }
    public void PushFrontSerializable(object obj) {
        if (obj == null) {
            return;
        }
        if (!obj.GetType().IsSerializable) {
            throw new Exception("Object [" + obj.GetType().Name + "] should be serializable!");
        }
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        byte[] barr = ms.ToArray();
        buff = ConcatByteArrays(BitConverter.GetBytes(barr.Length), barr, buff);
    } //PushFrontSerializable

    ////////////////////////////// Arrays
    public void Push(bool[] arr) {
        Push(arr.Length);
        byte ix = 0;
        byte bb = 0;
        for (int i = 0; i < arr.Length; ++i) {
            bb |= (byte)(arr[i] ? Math.Pow(2,ix) : 0);
            ++ix;
            if (ix > 7) {
                Push(bb);
                ix = 0;
                bb = 0;
            }
        }
        Push(bb);
    }
    public void Push(Byte[] arr) {
        Push(arr.Length);
        buff = ConcatByteArrays(buff, arr);
    }
    public void Push(SByte[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Int16[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Int32[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Int64[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(UInt16[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(UInt32[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(UInt64[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(float[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Double[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Char[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(DateTime[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(string[] arr) {
        Push(arr.Length);
        for (int i=0; i<arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Vector2[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Vector3[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(Quaternion[] arr) {
        Push(arr.Length);
        for (int i = 0; i < arr.Length; ++i) {
            Push(arr[i]);
        }
    }

    ////////////////////////////// Lists
    public void Push(List<bool> arr) {
        Push(arr.Count);
        byte ix = 0;
        byte bb = 0;
        for (int i = 0; i < arr.Count; ++i) {
            bb |= (byte)(arr[i] ? Math.Pow(2,ix) : 0);
            ++ix;
            if (ix > 7) {
                Push(bb);
                ix = 0;
                bb = 0;
            }
        }
        Push(bb);
    }
    public void Push(List<Byte> arr) {
        Push(arr.Count);
        buff = ConcatByteArrays(buff, arr.ToArray());
    }
    public void Push(List<SByte> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Int16> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Int32> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Int64> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<UInt16> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<UInt32> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<UInt64> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<float> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Double> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Char> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<DateTime> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<string> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Vector2> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Vector3> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }
    public void Push(List<Quaternion> arr) {
        Push(arr.Count);
        for (int i = 0; i < arr.Count; ++i) {
            Push(arr[i]);
        }
    }

    //--------------------------------------------------------------------------------------------------------
    public bool PopBool() {
        bool val = buff[index] == 1 ? true:false;
        ++index;
        return val;
    }
    public byte PopByte() {
        byte val = buff[index];
        ++index;
        return val;
    }
    public sbyte PopSByte() {
        sbyte val = (sbyte)buff[index];
        ++index;
        return val;
    }
    public Int16 PopInt16() {
        Int16 val = BitConverter.ToInt16(buff, index);
        index += 2;
        return val;
    }
    public Int32 PopInt32() {
        Int32 val = BitConverter.ToInt32(buff, index);
        index += 4;
        return val;
    }
    public Int64 PopInt64() {
        Int64 val = BitConverter.ToInt64(buff, index);
        index += 8;
        return val;
    }
    public UInt16 PopUInt16() {
        UInt16 val = BitConverter.ToUInt16(buff, index);
        index += 2;
        return val;
    }
    public UInt32 PopUInt32() {
        UInt32 val = BitConverter.ToUInt32(buff, index);
        index += 4;
        return val;
    }
    public UInt64 PopUInt64() {
        UInt64 val = BitConverter.ToUInt64(buff, index);
        index += 8;
        return val;
    }
    public float PopFloat(){
        float val = BitConverter.ToSingle(buff, index);
        index += sizeof(float);
        return val;
    }
    public double PopDouble() {
        double val = BitConverter.ToDouble(buff, index);
        index += sizeof(double); ;
        return val;
    }
    public char PopChar() {
        char val = BitConverter.ToChar(buff, index);
        index += 2;
        return val;
    }
    public DateTime PopDatetime() {
        DateTime val = DateTime.FromBinary(BitConverter.ToInt64(buff, index));
        index += 8;
        return val;
    }
    public string PopString() {
        int len = PopInt32();
        string val = Encoding.Unicode.GetString(buff, index, len);
        index += len;
        return val;
    }
    public string PopStringB64() {
        int len = PopInt32();
        string b64 = Encoding.UTF8.GetString(buff, index, len);
        byte[] barr = Convert.FromBase64String(b64);
        index += len;
        return Encoding.Unicode.GetString(barr, 0, barr.Length); ;
    }
    public Vector2 PopVector2() {
        float x = PopFloat();
        float y = PopFloat();
        return new Vector2(x, y);
    }
    public Vector3 PopVector3() {
        float x = PopFloat();
        float y = PopFloat();
        float z = PopFloat();
        return new Vector3(x, y, z);
    }
    public Quaternion PopQuaternion() {
        float x = PopFloat();
        float y = PopFloat();
        float z = PopFloat();
        float w = PopFloat();
        return new Quaternion(x, y, z, w);
    }
    public object PopSerializable() {
        int len = PopInt32();
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(buff, index, len);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj;
        try { obj = (object)binForm.Deserialize(memStream); }
        catch (Exception) {
            Console.WriteLine("ByteArrayToObject Error @ index " + index);
            return false;
        }
        index += len;
        return obj;
    }

    ////////////////////////////// Arrays
    public bool[] PopBoolArray() {
        int count = PopInt32();
        bool[] barr = new bool[count];
        byte tmp = PopByte();
        int ix = 0;
        for(int i = 0; i < count; ++i) {
            barr[i] = ((tmp & (byte)Math.Pow(2,ix)) > 0 ? true : false);
            ++ix;
            if(ix > 7) {
                ix = 0;
                tmp = PopByte();
            }
        }
        return barr;
    }
    public byte[] PopByteArray() {
        int count = PopInt32();
        byte[] barr = new byte[count];
        for(int i=0; i<count; ++i) {
            barr[i] = PopByte();
        }
        return barr;
    }
    public sbyte[] PopSByteArray() {
        int count = PopInt32();
        sbyte[] barr = new sbyte[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopSByte();
        }
        return barr;
    }
    public Int16[] PopInt16Array() {
        int count = PopInt32();
        Int16[] barr = new Int16[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopInt16();
        }
        return barr;
    }
    public Int32[] PopInt32Array() {
        int count = PopInt32();
        Int32[] barr = new Int32[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopInt32();
        }
        return barr;
    }
    public Int64[] PopInt64Array() {
        int count = PopInt32();
        Int64[] barr = new Int64[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopInt64();
        }
        return barr;
    }
    public UInt16[] PopUInt16Array() {
        int count = PopInt32();
        UInt16[] barr = new UInt16[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopUInt16();
        }
        return barr;
    }
    public UInt32[] PopUInt32Array() {
        int count = PopInt32();
        UInt32[] barr = new UInt32[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopUInt32();
        }
        return barr;
    }
    public UInt64[] PopUInt64Array() {
        int count = PopInt32();
        UInt64[] barr = new UInt64[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopUInt64();
        }
        return barr;
    }
    public float[] PopFloatArray() {
        int count = PopInt32();
        float[] barr = new float[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopFloat();
        }
        return barr;
    }
    public double[] PopDoubleArray() {
        int count = PopInt32();
        double[] barr = new double[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopDouble();
        }
        return barr;
    }
    public char[] PopCharArray() {
        int count = PopInt32();
        char[] barr = new char[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopChar();
        }
        return barr;
    }
    public DateTime[] PopDateTimeArray() {
        int count = PopInt32();
        DateTime[] barr = new DateTime[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopDatetime();
        }
        return barr;
    }
    public string[] PopStringArray() {
        int count = PopInt32();
        string[] barr = new string[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopString();
        }
        return barr;
    }
    public string[] PopB64Array() {
        int count = PopInt32();
        string[] barr = new string[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopStringB64();
        }
        return barr;
    }
    public Vector2[] PopVector2Array() {
        int count = PopInt32();
        Vector2[] barr = new Vector2[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopVector2();
        }
        return barr;
    }
    public Vector3[] PopVector3Array() {
        int count = PopInt32();
        Vector3[] barr = new Vector3[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopVector3();
        }
        return barr;
    }
    public Quaternion[] PopQuaternionArray() {
        int count = PopInt32();
        Quaternion[] barr = new Quaternion[count];
        for (int i = 0; i < count; ++i) {
            barr[i] = PopQuaternion();
        }
        return barr;
    }

    ////////////////////////////// Lists
    public List<bool> PopBoolList() {
        int count = PopInt32();
        List<bool> barr = new List<bool>(count);
        byte tmp = PopByte();
        int ix = 0;
        for (int i = 0; i < count; ++i) {
            barr.Add((tmp & (byte)Math.Pow(2, ix)) > 0 ? true : false);
            ++ix;
            if (ix > 7) {
                ix = 0;
                tmp = PopByte();
            }
        }
        return barr;
    }
    public List<byte> PopByteList() {
        int count = PopInt32();
        List<byte> lst = new List<byte>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopByte());
        }
        return lst;
    }
    public List<sbyte> PopSByteList() {
        int count = PopInt32();
        List<sbyte> lst = new List<sbyte>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopSByte());
        }
        return lst;
    }
    public List<Int16> PopInt16List() {
        int count = PopInt32();
        List<Int16> lst = new List<Int16>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopInt16());
        }
        return lst;
    }
    public List<Int32> PopInt32List() {
        int count = PopInt32();
        List<Int32> lst = new List<Int32>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopInt32());
        }
        return lst;
    }
    public List<Int64> PopInt64List() {
        int count = PopInt32();
        List<Int64> lst = new List<Int64>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopInt64());
        }
        return lst;
    }
    public List<UInt16> PopUInt16List() {
        int count = PopInt32();
        List<UInt16> lst = new List<UInt16>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopUInt16());
        }
        return lst;
    }
    public List<UInt32> PopUInt32List() {
        int count = PopInt32();
        List<UInt32> lst = new List<UInt32>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopUInt32());
        }
        return lst;
    }
    public List<UInt64> PopUInt64List() {
        int count = PopInt32();
        List<UInt64> lst = new List<UInt64>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopUInt64());
        }
        return lst;
    }
    public List<float> PopFloatList() {
        int count = PopInt32();
        List<float> lst = new List<float>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopFloat());
        }
        return lst;
    }
    public List<double> PopDoubleList() {
        int count = PopInt32();
        List<double> lst = new List<double>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopDouble());
        }
        return lst;
    }
    public List<char> PopCharList() {
        int count = PopInt32();
        List<char> lst = new List<char>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopChar());
        }
        return lst;
    }
    public List<DateTime> PopDateTimeList() {
        int count = PopInt32();
        List<DateTime> lst = new List<DateTime>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopDatetime());
        }
        return lst;
    }
    public List<string> PopStringList() {
        int count = PopInt32();
        List<string> lst = new List<string>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopString());
        }
        return lst;
    }
    public List<string> PopB64List() {
        int count = PopInt32();
        List<string> lst = new List<string>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopStringB64());
        }
        return lst;
    }
    public List<Vector2> PopVector2List() {
        int count = PopInt32();
        List<Vector2> lst = new List<Vector2>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopVector2());
        }
        return lst;
    }
    public List<Vector3> PopVector3List() {
        int count = PopInt32();
        List<Vector3> lst = new List<Vector3>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopVector3());
        }
        return lst;
    }
    public List<Quaternion> PopQuaternionList() {
        int count = PopInt32();
        List<Quaternion> lst = new List<Quaternion>(count);
        for (int i = 0; i < count; ++i) {
            lst.Add(PopQuaternion());
        }
        return lst;
    }

    //--------------------------------------------------------------------------------------------------------
    public string GetBase64() { return Convert.ToBase64String(buff); }
    public string GetString() { return Encoding.Unicode.GetString(buff); }
    public string GetHex(bool space=true) {
        string sRet = BitConverter.ToString(buff);
        if(space) sRet = sRet.Replace("-", " ");
        return sRet;
    } //GetHex


    //****************** Statics *********************//

    //--------------------------------------------------------------------------------------------------------
    public static byte[] ConcatByteArrays(byte[] first, byte[] second) {
        byte[] ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }
    public static byte[] ConcatByteArrays(byte[] first, byte[] second, byte[] third) {
        byte[] ret = new byte[first.Length + second.Length + third.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        Buffer.BlockCopy(third, 0, ret, first.Length + second.Length, third.Length);
        return ret;
    } 
    public static byte[] ConcatByteArrays(params byte[][] arrays) {
        byte[] ret = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;
        for (int i = 0; i < arrays.Length; i++) {
            Buffer.BlockCopy(arrays[i], 0, ret, offset, arrays[i].Length);
            offset += arrays[i].Length;
        }
        return ret;
    } //ConcatByteArrays

    //--------------------------------------------------------------------------------------------------------
    public static byte[] SubArray(byte[] data, int index) {
        if (index > data.Length || index < 0) {
            return null;
        }
        int length = data.Length-index;
        byte[] result = new byte[length];
        Buffer.BlockCopy(data, index, result, 0, length);
        return result;
    } //SubArray
    public static byte[] SubArray(byte[] data, int index, int length) {
        if (index + length > data.Length) {
            return null;
        }
        byte[] result = new byte[length];
        Buffer.BlockCopy(data, index, result, 0, length);
        return result;
    } //SubArray

    //--------------------------------------------------------------------------------------------------------
    public static byte WrapBoolsIntoByte(params bool[] bools) {
        byte ix = 0;
        byte bb = 0;
        for (int i = 0; i < bools.Length; ++i) {
            bb |= (byte)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 7)  return bb;
        }
        return bb;
    }
    public static byte WrapBoolsIntoByte(List<bool> bools) {
        byte ix = 0;
        byte bb = 0;
        for (int i = 0; i < bools.Count; ++i) {
            bb |= (byte)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 7) return bb;
        }
        return bb;
    }
    public static UInt16 WrapBoolsIntoShort(params bool[] bools) {
        int ix = 0;
        UInt16 bb = 0;
        for (int i = 0; i < bools.Length; ++i) {
            bb |= (UInt16)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 15) return bb;
        }
        return bb;
    }
    public static UInt16 WrapBoolsIntoShort(List<bool> bools) {
        int ix = 0;
        UInt16 bb = 0;
        for (int i = 0; i < bools.Count; ++i) {
            bb |= (UInt16)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 15) return bb;
        }
        return bb;
    }
    public static UInt32 WrapBoolsIntoInt(params bool[] bools) {
        int ix = 0;
        UInt32 bb = 0;
        for (int i = 0; i < bools.Length; ++i) {
            bb |= (UInt32)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 31) return bb;
        }
        return bb;
    }
    public static UInt32 WrapBoolsIntoInt(List<bool> bools) {
        int ix = 0;
        UInt32 bb = 0;
        for (int i = 0; i < bools.Count; ++i) {
            bb |= (UInt32)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 31) return bb;
        }
        return bb;
    }
    public static UInt64 WrapBoolsIntoLong(params bool[] bools) {
        int ix = 0;
        UInt64 bb = 0;
        for (int i = 0; i < bools.Length; ++i) {
            bb |= (UInt64)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 63) return bb;
        }
        return bb;
    }
    public static UInt64 WrapBoolsIntoLong(List<bool> bools) {
        int ix = 0;
        UInt64 bb = 0;
        for (int i = 0; i < bools.Count; ++i) {
            bb |= (UInt64)(bools[i] ? Math.Pow(2, ix) : 0);
            ++ix;
            if (ix > 63) return bb;
        }
        return bb;
    }
    public static bool UnwrapBoolFrom(byte val, int pos) {
        if (pos > 7) return false;
        return ((val & (byte)Math.Pow(2,pos)) > 0? true : false);
    }
    public static bool UnwrapBoolFrom(UInt16 val, int pos) {
        if (pos > 15) return false;
        return ((val & (byte)Math.Pow(2, pos)) > 0 ? true : false);
    }
    public static bool UnwrapBoolFrom(UInt32 val, int pos) {
        if (pos > 31) return false;
        return ((val & (byte)Math.Pow(2, pos)) > 0 ? true : false);
    }
    public static bool UnwrapBoolFrom(UInt64 val, int pos) {
        if (pos > 63) return false;
        return ((val & (byte)Math.Pow(2, pos)) > 0 ? true : false);
    }

} //CLASS NetMessage