using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Managers;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public class MeleeEnemy : Enemy
    {
        
        protected override void AttackPlayer()
        {
            throw new NotImplementedException();
        }

        //float step = moveSpeed * Time.deltaTime;

        // Check if which value is closer to the player in the x axis for the initial movement
        //float distanceZig = Vector2.Distance(GetPosition(), _zigPosition);
        //float distanceZag = Vector2.Distance(GetPosition(), _zagPosition);
        /*if (distanceZig >= distanceZag)
        {
            // Move to ZAG since it's closer
            transform.position = Vector3.MoveTowards(GetPosition(), _zagPosition, step);
        }
        else
        {
            // Move to ZIG since it's closer
            transform.position = Vector3.MoveTowards(GetPosition(), _zigPosition, step);
        }*/


        /*
        *PSEUDOCODIGO TIME!!!
        Zigzag:
        - Enemigo spawnea.
        - Escoje 2 valores (a y b) en rango [-3.5, 3.5]. Seran nuestros x's.
        - Sea a el minimo de ambos valores, entonces (a+2) <= b (diferencia de 2 para que sea valido. Si no, escoja otro par)
        - Elije el valor que esté mas cerca de él, y se mueve ahi (e.g. Move(a, y)) = ZIG
        - Calcule el valor del ZAG dependiendo si esta arriba o abajo:
        --- Si esta arriba, z = Random.Range[0.5,1]. Entonces Move(b, y-z)
        --- Si esta abajo, z = Random.Range[0.5,1]. Entonces Move(b, y+z)
        - Hacer lo mismo del ZAG para calcular el siguiente ZIG.
        - Revisa si en el siguiente movimiento:
        --- Si esta arriba, si y <= 2.5, pare y haga rush.
        --- Si esta abajo, si y >= 2.5, pare y haga rush.
         **/ 
      

    }
}
