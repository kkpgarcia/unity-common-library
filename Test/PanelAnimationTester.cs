using UnityEngine;
using System.Collections;
using Common.UI;

namespace Common.Test {
    public class PanelAnimationTester : MonoBehaviour {
        public Panel Panel;

        public void Start() {
            StartCoroutine(Test());
        }

        private IEnumerator Test() {
            Panel.SetPosition("Hide");
            yield return new WaitForSeconds(1);
            Panel.SetPosition("Show");
        }
    }
}