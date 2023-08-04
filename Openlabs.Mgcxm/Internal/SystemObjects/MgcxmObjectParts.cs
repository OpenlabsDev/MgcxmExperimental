// Copr. (c) Nexus 2023. All rights reserved.

using System.Security.AccessControl;

namespace Openlabs.Mgcxm.Internal.SystemObjects;

public class MgcxmArray<T> : MgcxmObject<T[]>
{
    public MgcxmArray(T[] obj) : base(obj) {}
    
    public static implicit operator T[](MgcxmArray<T> obj) => obj.Retrieve();
    public static implicit operator MgcxmArray<T>(T[] obj) => new(obj);
}

public class MgcxmChar : MgcxmObject<char>
{
    public MgcxmChar(char obj) : base(obj) {}
    
    public static implicit operator char(MgcxmChar obj) => obj.Retrieve();
    public static implicit operator MgcxmChar(char obj) => new(obj);
}

public class MgcxmString : MgcxmObject<string>
{
    public MgcxmString(string obj) : base(obj) {}
    
    public static implicit operator string(MgcxmString obj) => obj.Retrieve();
    public static implicit operator MgcxmString(string obj) => new(obj);
}

public class MgcxmInt : MgcxmObject<int>
{
    public MgcxmInt(int obj) : base(obj) {}
    
    public static implicit operator int(MgcxmInt obj) => obj.Retrieve();
    public static implicit operator MgcxmInt(int obj) => new(obj);
}

public class MgcxmUint : MgcxmObject<uint>
{
    public MgcxmUint(uint obj) : base(obj) {}
    
    public static implicit operator uint(MgcxmUint obj) => obj.Retrieve();
    public static implicit operator MgcxmUint(uint obj) => new(obj);
}

public class MgcxmLong : MgcxmObject<long>
{
    public MgcxmLong(long obj) : base(obj) {}
    
    public static implicit operator long(MgcxmLong obj) => obj.Retrieve();
    public static implicit operator MgcxmLong(long obj) => new(obj);
}

public class MgcxmUlong : MgcxmObject<ulong>
{
    public MgcxmUlong(ulong obj) : base(obj) { }
    
    public static implicit operator ulong(MgcxmUlong obj) => obj.Retrieve();
    public static implicit operator MgcxmUlong(ulong obj) => new(obj);
}

public class MgcxmShort : MgcxmObject<short>
{
    public MgcxmShort(short obj) : base(obj) { }
    
    public static implicit operator short(MgcxmShort obj) => obj.Retrieve();
    public static implicit operator MgcxmShort(short obj) => new(obj);
}

public class MgcxmUshort : MgcxmObject<ushort>
{
    public MgcxmUshort(ushort obj) : base(obj) { }
    
    public static implicit operator ushort(MgcxmUshort obj) => obj.Retrieve();
    public static implicit operator MgcxmUshort(ushort obj) => new(obj);
}