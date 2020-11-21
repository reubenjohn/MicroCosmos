using UnityEngine;

namespace Genealogy
{
    public interface IChoreographerListener
    {
        void OnSwitchCamera(Camera cam);
    }
}