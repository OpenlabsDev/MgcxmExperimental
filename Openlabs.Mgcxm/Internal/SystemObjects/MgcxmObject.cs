// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal.SystemObjects;

public abstract class MgcxmObjectBase { }

public class MgcxmObject<T> : MgcxmObjectBase, IMgcxmSystemObject
{
    // ctors
    public MgcxmObject(T obj)
    {
        _id = obj.GetHashCode();
        _value = obj;
        
        SafeInvocation.InvokeSafe(() => MgcxmObjectManager.Register(_id, this._value));
    }
    ~MgcxmObject() => Trash();
    
    // ops
    public static implicit operator T(MgcxmObject<T> obj) => obj.Retrieve();
    public static implicit operator MgcxmObject<T>(T obj) => new(obj);

    // methods
    protected T Retrieve() => MgcxmObjectManager.Retrieve<T>(_id);
    public void Trash() => SafeInvocation.InvokeSafe(() => MgcxmObjectManager.Deregister(_id));
    
    // vars
    public MgcxmId AllocatedId => _id;
    private MgcxmId _id;
    
    public T Value => _value;
    private T _value;
}