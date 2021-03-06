/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace SOATest
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Work : TBase
  {
    private int _num1;
    private int _num2;
    private Operation _op;

    public int Num1
    {
      get
      {
        return _num1;
      }
      set
      {
        __isset.num1 = true;
        this._num1 = value;
      }
    }

    public int Num2
    {
      get
      {
        return _num2;
      }
      set
      {
        __isset.num2 = true;
        this._num2 = value;
      }
    }

    /// <summary>
    /// 
    /// <seealso cref="Operation"/>
    /// </summary>
    public Operation Op
    {
      get
      {
        return _op;
      }
      set
      {
        __isset.op = true;
        this._op = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool num1;
      public bool num2;
      public bool op;
    }

    public Work() {
      this._num1 = 0;
      this.__isset.num1 = true;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.I32) {
                Num1 = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I32) {
                Num2 = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.I32) {
                Op = (Operation)iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("Work");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.num1) {
          field.Name = "num1";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(Num1);
          oprot.WriteFieldEnd();
        }
        if (__isset.num2) {
          field.Name = "num2";
          field.Type = TType.I32;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(Num2);
          oprot.WriteFieldEnd();
        }
        if (__isset.op) {
          field.Name = "op";
          field.Type = TType.I32;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32((int)Op);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("Work(");
      bool __first = true;
      if (__isset.num1) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Num1: ");
        __sb.Append(Num1);
      }
      if (__isset.num2) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Num2: ");
        __sb.Append(Num2);
      }
      if (__isset.op) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Op: ");
        __sb.Append(Op);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
