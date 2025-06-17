using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNameGenerator : ScriptableObject
{
    public static class UnitNameGenerator
    {
        private static readonly string[] Names = new string[]
  {
    "Иванов И.И.",
    "Петров П.П.",
    "Сидоров С.С.",
    "Кузнецов К.К.",
    "Смирнов С.М.",
    "Васильев В.В.",
    "Попов П.О.",
    "Новиков Н.Н.",
    "Федоров Ф.Ф.",
    "Морозов М.М.",
    "Волков В.Л.",
    "Алексеев А.А.",
    "Лебедев Л.Л.",
    "Семенов С.С.",
    "Егоров Е.Е.",
    "Павлов П.П.",
    "Козлов К.К.",
    "Степанов С.С.",
    "Николаев Н.Н.",
    "Орлов О.О.",
    "Андреев А.А.",
    "Макаров М.М.",
    "Никитин Н.Н.",
    "Захаров З.З.",
    "Зайцев З.З.",
    "Соловьев С.С.",
    "Борисов Б.Б.",
    "Яковлев Я.Я.",
    "Григорьев Г.Г.",
    "Романов Р.Р."
  };

        public static string GetRandomUnitName()
        {
            System.Random random = new System.Random();
            int index = random.Next(Names.Length);
            return Names[index];
        }

        public static string GetRandomNumber()
        {
            int index = Convert.ToInt32(UnityEngine.Random.value * 999);
            return index.ToString();
        }
    }
}
