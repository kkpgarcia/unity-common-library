using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Common.Reactive
{
    public class ReactiveTest : MonoBehaviour
    {
        public Observable<int> observableInt;
        public Observable<string> observableString;

        public TMP_InputField someField;
        
        public void Start()
        {
            observableInt = new Observable<int>(0);
            observableInt
                .GetDataStream()
                .Subscribe((s, o) =>
                {
                    int value = (int)o;
                    Debug.Log("Value changed to " + value);
                });
            this.observableInt.Value = 10;

            observableString = new Observable<string>("");
            
            observableString
                .GetDataStream()
                .Subscribe((s, o) =>
                {
                    string value = (string)o;
                    Debug.Log("Value changed to " + value);

                    // this.label.value = value;
                    this.someField.text = value;
                });
            
            this.someField.onValueChanged.AddListener((v) => observableString.Value = v);

            observableString.Value = "Hello world!";
        }

        private void OnDestroy()
        {
            
        }
    }
}