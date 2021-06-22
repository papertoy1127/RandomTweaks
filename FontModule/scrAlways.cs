using System;
using System.IO;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksFontModule {
    public class scrAlways : MonoBehaviour {
        private void Update() {
            StartCoroutine(Patch.Patch.UpdateFontCo());
        }
    }
}