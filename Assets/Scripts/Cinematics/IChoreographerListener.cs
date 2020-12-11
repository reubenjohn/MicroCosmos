using UnityEngine;

namespace Cinematics
{
    public interface IChoreographerListener
    {
        void OnSwitchCamera(Camera cam);
    }
}