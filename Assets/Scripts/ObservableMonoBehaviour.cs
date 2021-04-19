using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class ObservableMonoBehaviour<T> : MonoBehaviourPun, IPunObservable where T : Component
{

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        IEnumerable<PropertyInfo> properties = typeof(T).GetProperties().Where(p => p.IsDefined(typeof(ObservedAttribute)));
        if (stream.IsWriting)
        {
            foreach(PropertyInfo property in properties)
            {
                stream.SendNext(property.GetValue(this));
            }
        }
        else
        {
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(this, stream.ReceiveNext());
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public class ObservedAttribute : Attribute
{

}