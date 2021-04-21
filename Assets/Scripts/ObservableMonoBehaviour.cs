using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Properties of subclasses with the <see cref="ObservedAttribute"/> will be synced with PUN clients
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObservableMonoBehaviour<T> : MonoBehaviourPun, IPunObservable where T : ObservableMonoBehaviour<T>
{
    /// <summary><c>true</c> if this object should sync properties</summary>
    protected bool isSending = true;

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        IEnumerable<PropertyInfo> properties = typeof(T).GetProperties().Where(p => p.IsDefined(typeof(ObservedAttribute)));
        if (stream.IsWriting)
        {
            if (isSending)
            {
                foreach(PropertyInfo property in properties)
                {
                    stream.SendNext(property.GetValue(this));
                }
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

/// <summary>
/// Add this <see cref="Attribute"/> to Properties that should be synced through <see cref="IPunObservable"/><br/>
/// Ensure property has both a getter and setter
/// </summary>
/// <remarks>
/// See <see cref="ObservableMonoBehaviour{t}"/>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public class ObservedAttribute : Attribute
{ }