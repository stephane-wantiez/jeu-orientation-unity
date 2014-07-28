using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReperesManager : MonoBehaviour
{
    private static ReperesManager _instance;
    public static ReperesManager Instance { get { return _instance; } }

    public RepereType[] typesReperes;

    private Dictionary<RepereType.TypeCouleur, RepereType> reperesPerColor = new Dictionary<RepereType.TypeCouleur, RepereType>();
    private Dictionary<RepereType.TypePointCardinal, RepereType> reperesPerCard = new Dictionary<RepereType.TypePointCardinal, RepereType>();
    public RepereType.TypeEnum type;

    public Transform repereHautPosition;
    public Transform repereDroitePosition;
    public Transform repereBasPosition;
    public Transform repereGauchePosition;

    private RepereType repereHaut;
    private RepereType repereDroite;
    private RepereType repereBas;
    private RepereType repereGauche;

    void Awake()
    {
        _instance = this;
        initReperesTypes();
    }

    private void initReperesTypes()
    {
        reperesPerColor.Clear();
        reperesPerCard.Clear();

        if (typesReperes != null)
        {
            foreach(RepereType typeRepere in typesReperes)
            {
                if (typeRepere.type == RepereType.TypeEnum.Couleur)
                {
                    reperesPerColor.Add(typeRepere.typeCouleur, typeRepere);
                }
                else // type = PointCardinal
                {
                    reperesPerCard.Add(typeRepere.typePointCardinal, typeRepere);
                }
            }
        }
    }

    public RepereType getRandomRepereType()
    {
        int randomValue = UnityEngine.Random.Range(0, RepereType.NB_TYPE_TOUS);

        if (type == RepereType.TypeEnum.Couleur)
        {
            RepereType.TypeCouleur randomColor = (RepereType.TypeCouleur)randomValue;
            return reperesPerColor[randomColor];
        }
        else
        {
            RepereType.TypePointCardinal randomCard = (RepereType.TypePointCardinal)randomValue;
            return reperesPerCard[randomCard];
        }
    }

    private void initReperesWithRandomColors()
    {
        RepereType.TypeCouleur[] colors = new RepereType.TypeCouleur[RepereType.NB_TYPE_REGULIER];
        for (int i = 0; i < RepereType.NB_TYPE_REGULIER; ++i)
        {
            colors[i] = (RepereType.TypeCouleur)i;
        }
        Utils.ShuffleFY(colors);

        repereHaut   = reperesPerColor[colors[0]];
        repereDroite = reperesPerColor[colors[1]];
        repereBas    = reperesPerColor[colors[2]];
        repereGauche = reperesPerColor[colors[3]];
    }

    private void initReperesWithRandomPointCardinal()
    {
        int pointCardinalHautValue = UnityEngine.Random.Range(0, RepereType.NB_TYPE_REGULIER);
        int pointCardinalDroiteValue = (pointCardinalHautValue + 1) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalBasValue    = (pointCardinalHautValue + 2) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalGaucheValue = (pointCardinalHautValue + 3) % RepereType.NB_TYPE_REGULIER;

        repereHaut   = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalHautValue  ];
        repereDroite = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalDroiteValue];
        repereBas    = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalBasValue   ];
        repereGauche = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalGaucheValue];
    }

    public void initReperes()
    {
        if (type == RepereType.TypeEnum.Couleur)
        {
            initReperesWithRandomColors();
        }
        else
        {
            initReperesWithRandomPointCardinal();
        }
    }

    private void generateRepere(RepereType repereType, Transform reperePosition)
    {
        Quaternion repereRotation = repereType.rotatePrefabAtGeneration ? reperePosition.rotation : Quaternion.identity;
        GameObject repereObject = Instantiate(repereType.reperePrefabHaut, reperePosition.position, repereRotation) as GameObject;
        repereObject.transform.parent = reperePosition;
    }

    private void clearReperesParents()
    {
        Utils.DeleteChildrenOfTransform(repereHautPosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereDroitePosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereBasPosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereGauchePosition, DestroyImmediate);
    }

    public void generateReperes()
    {
        clearReperesParents();
        generateRepere(repereHaut, repereHautPosition);
        generateRepere(repereDroite, repereDroitePosition);
        generateRepere(repereBas, repereBasPosition);
        generateRepere(repereGauche, repereGauchePosition);
    }
}
