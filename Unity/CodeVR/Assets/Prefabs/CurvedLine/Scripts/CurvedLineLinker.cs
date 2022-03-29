// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CurvedLineLinker : MonoBehaviour
// {
//     public enum ConnectionType {
//         Start,
//         End
//     }

//     [SerializeField] private CurvedLine _line;

//     [SerializeField] private ConnectionType _connectionType;

//     // Start is called before the first frame update
//     void Start()
//     {
//         if (_connectionType == ConnectionType.Start)
//             this._line.StartTransform = this.gameObject;
//         if (_connectionType == ConnectionType.End)
//             this._line.EndTransform = this.gameObject;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (_connectionType == ConnectionType.Start)
//             this._line.StartTransform = this.gameObject;
//         if (_connectionType == ConnectionType.End)
//             this._line.EndTransform = this.gameObject;
//     }
// }
