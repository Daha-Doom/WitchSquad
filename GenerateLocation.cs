using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GeneratedBlocks
{
    public bool[] inOut;
    public bool[,] tile;

    public GeneratedBlocks (int countX, int countY)
    {
        inOut = new bool[8];
        tile = new bool[countX, countY];

        for (int i = 0; i < 8; i++)
            inOut[i] = false;

        for (int i = 0; i < countX; i++)
            for (int j = 0; j < countY; j++)
                if (i == 0 || j == 0)
                    tile[i, j] = true;
                else tile[i, j] = false;
    }

    public GeneratedBlocks (bool[] inOut, bool[,] tile)
    {
        this.inOut = inOut;
        this.tile = tile;
    }

    public void SetBoolTile(int i, int j, bool value)
    {
        tile[i, j] = value;
    }

    public void SetBoolInOut (int i, bool value)
    {
        inOut[i] = value;
    }

    public bool GetBoolInOut(int i)
    {
        return inOut[i];
    }

    public bool GetBoolTile(int i, int j)
    {
        return tile[i, j];
    }
}

public class GenerateLocation : MonoBehaviour
{
    [SerializeField] Blocks blocks;
    SpriteSetGreenMaze greenMaze;
    SpriteRenderer spriteRenderer;
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();
    [SerializeField] int wightBlocks;
    [SerializeField] int hightBlocks;
    [SerializeField] float startPositionX;
    [SerializeField] float startPositionY;
    [SerializeField] GameObject tileGround;
    [SerializeField] Transform enemy;

    int wight, hight, yEnd;
    int wightBlock, hightBlock;
    int countBlocksX, countBlocksY;
    float startPointX, startPointY;
    [SerializeField] float hX = 1f, hY = 1f;

    Dictionary<int, bool> fillSprite = new Dictionary<int, bool>();

    bool top, bottom, left, right, topLeft, topRight, bottomLeft, bottomRight;
    const char up = 'u', down = 'd', rig = 'r', lef = 'l';

    void Awake()
    {
        for (int i = 0; i < 8; i++)
            fillSprite.Add(i, false);

        greenMaze = GetComponent<SpriteSetGreenMaze>();

        wightBlock = Convert.ToInt32(blocks.block[0].transform.localScale.x);
        hightBlock = Convert.ToInt32(blocks.block[0].transform.localScale.y);

        wight = Convert.ToInt32(transform.localScale.x);
        hight = Convert.ToInt32(transform.localScale.y);

        countBlocksX = wight / 10;
        countBlocksY = hight / 10;

        startPointX = -44;
        startPointY = -45;

        //startPointX = (0 - wight / 2f) + wightBlock / 2f;
        //startPointY = (0 - hight / 2f) + hightBlock / 2f;

        GenerateCells();
        UpdateSprite();
        SpawnObject();
    }

    private void GenerateCells()
    {
        System.Random random = new System.Random();

        int i = 0, j = 0;
        bool leftTurn = true, downTurn = true;

        List<char> path = new List<char>();
        List<Tuple<int, int>> pathID = new List<Tuple<int, int>>();

        GeneratedBlocks[,] generatedBlocks = new GeneratedBlocks[countBlocksX, countBlocksY];

        //Построение пути
        while (true)
        {
            int pathTurn = random.Next(0, 4);

            switch (pathTurn)
            {
                //Вверх
                case 0:
                    if (j < (countBlocksY - 1) && ((path.Count > 0) ? (path[path.Count - 1] != down) : true) /*&& !pathID.Contains(Tuple.Create(i,j))*/)
                    {
                        downTurn = true;
                        path.Add(up);
                        pathID.Add(Tuple.Create(i, j));
                        j++;
                    }
                break;

                    //Вправо
                case 1:
                    if (i < (countBlocksX - 1) && ((path.Count > 0) ? (path[path.Count - 1] != lef) : true) /*&& !pathID.Contains(Tuple.Create(i, j))*/)
                    {
                        leftTurn = true;
                        path.Add(rig);
                        pathID.Add(Tuple.Create(i, j));
                        i++;
                    }
                break;

                    //Вниз
                case 2:
                    if (j > 1 && i < (countBlocksX - 1) && ((path.Count > 0) ? ((path[path.Count - 1] != up) && (path[path.Count - 1] != down)) : true) && downTurn/*&& !pathID.Contains(Tuple.Create(i, j))*/)
                    {
                        downTurn = false;
                        path.Add(down);
                        pathID.Add(Tuple.Create(i, j));
                        j--;
                    }
                break;

                    //Влево
                case 3:
                    if (i > 1 && j < (countBlocksY - 1) && ((path.Count > 0) ? ((path[path.Count - 1] != rig) && (path[path.Count - 1] != lef)) : true && leftTurn) /*&& !pathID.Contains(Tuple.Create(i, j))*/)
                    {
                        leftTurn = false;
                        path.Add(lef);
                        pathID.Add(Tuple.Create(i, j));
                        i--;
                    }
                break;
            }

            if (i == (countBlocksX - 1) && (j == (countBlocksY - 3) || j == (countBlocksY - 2) || j == (countBlocksY - 1)))
            {
                yEnd = j;

                path.Add(rig);
                pathID.Add(Tuple.Create(i, j));

                break;
            }

            //stopK++;
            //if (stopK > 1000)
            //    break;
        }

        //DEGUG
        string p = "";
        for (i = 0; i < path.Count; i++) 
            p += path[i].ToString() + " ";
        Debug.Log(p);
        //DEBUG

        //Заполнение ячеек
        for (j = 0; j < countBlocksY; j++)
            for (i = 0; i < countBlocksX; i++)
            {
                int interCount = 0;

                generatedBlocks[i, j] = new GeneratedBlocks(wightBlocks, hightBlocks);

                //if (i == 0 && j == 9)
                //    i = i;

                //Если это стартовый блок
                if (i == 0 && j == 0)
                {
                    generatedBlocks[i, j].SetBoolInOut(0, true);

                    if (path[i] == rig)
                        generatedBlocks[i, j].SetBoolInOut(random.Next(4, 6), true); // выход справа
                    else
                        generatedBlocks[i, j].SetBoolInOut(random.Next(2, 4), true); // выход сверху

                    interCount += 2;
                }

                //Если блок является частью основного маршрута
                else if (pathID.Contains(Tuple.Create(i, j)))
                {
                    int IdPath = pathID.IndexOf(Tuple.Create(i, j));

                    switch (path[IdPath - 1])
                    {
                        case up: // вход снизу
                            if (generatedBlocks[i, j - 1].GetBoolInOut(2))
                            {
                                generatedBlocks[i, j].SetBoolInOut(7, true);
                                interCount++;
                            }
                            if (generatedBlocks[i, j - 1].GetBoolInOut(3))
                            {
                                generatedBlocks[i, j].SetBoolInOut(6, true);
                                interCount++;
                            }
                            break;

                        case rig: // вход слева
                            if (generatedBlocks[i - 1, j].GetBoolInOut(4))
                            {
                                generatedBlocks[i, j].SetBoolInOut(1, true);
                                interCount++;
                            }
                            if (generatedBlocks[i - 1, j].GetBoolInOut(5))
                            {
                                generatedBlocks[i, j].SetBoolInOut(0, true);
                                interCount++;
                            }
                            break;

                        case down: // вход сверху
                            if (generatedBlocks[i, j - 1].GetBoolInOut(7))
                            {
                                generatedBlocks[i, j].SetBoolInOut(2, true);
                                interCount++;
                            }
                            if (generatedBlocks[i, j - 1].GetBoolInOut(6))
                            {
                                generatedBlocks[i, j].SetBoolInOut(3, true);
                                interCount++;
                            }
                            break;

                        case lef: // вход справа
                            if (generatedBlocks[i - 1, j].GetBoolInOut(1))
                            {
                                generatedBlocks[i, j].SetBoolInOut(4, true);
                                interCount++;
                            }
                            if (generatedBlocks[i - 1, j].GetBoolInOut(0))
                            {
                                generatedBlocks[i, j].SetBoolInOut(5, true);
                                interCount++;
                            }
                            break;
                    }

                    //Если это не последняя ячейка основного пути
                    if (IdPath != path.Count - 1)
                        switch (path[IdPath])
                        {
                            case down: // выход снизу
                                {
                                    generatedBlocks[i, j].SetBoolInOut(random.Next(7, 8), true);
                                    interCount++;
                                }
                                break;

                            case lef: // выход слева
                                {
                                    generatedBlocks[i, j].SetBoolInOut(random.Next(0, 2), true);
                                    interCount++;
                                }
                                break;

                            case up: // выход сверху
                                {
                                    generatedBlocks[i, j].SetBoolInOut(random.Next(2, 4), true);
                                    interCount++;
                                }
                                break;

                            case rig: // выход справа
                                {
                                    generatedBlocks[i, j].SetBoolInOut(random.Next(4, 6), true);
                                    interCount++;
                                }
                                break;
                        }

                    //Создаём выход если это блок выхода с уровня
                    if (i == countBlocksX - 1 && j == yEnd)
                        generatedBlocks[i, j].SetBoolInOut(5, true);
                }

                //Проверяем выходы предыдущих блоков
                if (i != 0 || j != 0)
                {
                    if (j != 0)
                    {
                        //Проверка на вход снизу
                        if (generatedBlocks[i, j - 1].GetBoolInOut(3))
                        {
                            generatedBlocks[i, j].SetBoolInOut(6, true);
                            interCount++;
                        }

                        if (generatedBlocks[i, j - 1].GetBoolInOut(2))
                        {
                            generatedBlocks[i, j].SetBoolInOut(7, true);
                            interCount++;
                        }
                    }

                    if (i != 0)
                    {
                        //Проверка на вход слева
                        if (generatedBlocks[i - 1, j].GetBoolInOut(4))
                        {
                            generatedBlocks[i, j].SetBoolInOut(1, true);
                            interCount++;
                        }

                        if (generatedBlocks[i - 1, j].GetBoolInOut(5))
                        {
                            generatedBlocks[i, j].SetBoolInOut(0, true);
                            interCount++;
                        }
                    }
                }

                int inOutCount = random.Next(2, 5);

                //Распределение дополнительных выходов
                if ((j != countBlocksY - 1 || i != countBlocksX - 1) && interCount < inOutCount)
                    //while (interCount != inOutCount)
                    for (int k = interCount; k < inOutCount; k++)
                    {
                        //Доп выходы создаются только справа и сверху
                        int randInOut;
                        //левый нижни         //касается нижней стены   //касается левой стены                      // не касается края
                        if ((i == 0 && j == 0) || (j == 0) || (i == 0) || (i != 0 && j != 0 && i != countBlocksX - 1 && j != countBlocksY - 1))
                            randInOut = random.Next(2, 6);

                        // правый нижний              // касается правой стены
                        else if ((i == 0 && j == countBlocksY - 1) || (i == countBlocksX - 1))
                            randInOut = random.Next(2, 4);

                        // касается верхней стены           // левый верхний
                        else if ((j == countBlocksY - 1) || (i == countBlocksX - 1 && j == 0))
                            randInOut = random.Next(4, 6);

                        // верхний правый
                        else break;

                        //проверка на существование входа\выхода
                        if (!generatedBlocks[i, j].GetBoolInOut(randInOut))
                        {
                            generatedBlocks[i, j].SetBoolInOut(randInOut, true);
                            interCount++;
                        }
                    }

                //Удаление тайлов входа\выхода из матрицы
                for (int k = 0; k < 8; k++)
                    if (generatedBlocks[i, j].GetBoolInOut(k))
                        switch (k)
                        {
                            case 0:
                                generatedBlocks[i, j].SetBoolTile(0, 1, false);
                                generatedBlocks[i, j].SetBoolTile(0, 2, false);
                                break;

                            case 1:
                                generatedBlocks[i, j].SetBoolTile(0, hightBlock - 2, false);
                                generatedBlocks[i, j].SetBoolTile(0, hightBlock - 3, false);
                                break;

                            case 2:
                                generatedBlocks[i, j].SetBoolTile(2, hightBlocks - 1, false);
                                generatedBlocks[i, j].SetBoolTile(3, hightBlocks - 1, false);
                                break;

                            case 3:
                                generatedBlocks[i, j].SetBoolTile(wightBlock - 2, hightBlocks - 1, false);
                                generatedBlocks[i, j].SetBoolTile(wightBlock - 3, hightBlocks - 1, false);
                                break;

                            case 4:
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 1, hightBlock - 1, false);
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 1, hightBlock - 2, false);
                                break;

                            case 5:
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 1, 1, false);
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 1, 2, false);
                                break;

                            case 6:
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 2, 0, false);
                                generatedBlocks[i, j].SetBoolTile(wightBlocks - 3, 0, false);
                                break;

                            case 7:
                                generatedBlocks[i, j].SetBoolTile(2, 0, false);
                                generatedBlocks[i, j].SetBoolTile(3, 0, false);
                                break;
                        }

                ////Выбор стратегии генерации
                //int stratGen = random.Next(1, 3);

                //switch (stratGen)
                //{
                //    case 1:
                //        PyramidStrategy(generatedBlocks[i,j]);
                //        break;
                //    case 2:
                //        GridStrategy(generatedBlocks[i,j]);
                //        break;
                //    case 3:
                //        ClimbWallStrategy(generatedBlocks[i,j]);
                //        break;
                //}
            }

        //Отрисовка карты
        DrawMap(generatedBlocks);


        FillCells(generatedBlocks);
    }

    private void FillCells(GeneratedBlocks[,] generatedBlocks)
    {
        System.Random random = new System.Random();

        for (int j = 0, yPos = Convert.ToInt32(startPointY); j < countBlocksX; j++, yPos += 10)
            for (int i = 0, xPos = Convert.ToInt32(startPointX); i < countBlocksY; i++, xPos += 10)
            {
                bool notComferd = true;

                while (notComferd)
                {
                    int idBlock = random.Next(0, blocks.block.Count);

                    for (int k = 0; k < 8; k++)
                    {
                        if (generatedBlocks[i, j].GetBoolInOut(k) == true && blocks.block[idBlock].GetComponent<Block>().exit[k] == false)
                            break;

                        if (k == 7)
                        {
                            notComferd = false;

                            Transform clone = Instantiate(blocks.block[idBlock].transform, new Vector3(xPos, yPos, 10), Quaternion.identity);

                            clone.SetParent(this.transform);
                        }
                    }
                }
            }
    }

    private void PyramidStrategy (GeneratedBlocks generatedBlock)
    {
        bool leftUpCorner = false, rightUpCorner = false, leftDownCorner = false, rightDownCorner = false;
        
        for (int k = 0; k < 8; k++)
            if (generatedBlock.GetBoolInOut(k))
                switch (k)
                {
                    case 0:
                        leftDownCorner = true;
                        break;
                    case 1:
                        generatedBlock.SetBoolTile(1, 2, true);
                        leftUpCorner = true;
                        break;
                    case 2:
                        generatedBlock.SetBoolTile(2, 3, true);
                        leftUpCorner = true;
                        break;
                    case 3:
                        generatedBlock.SetBoolTile(7, 3, true);
                        rightUpCorner = true;
                        break;
                    case 4:
                        generatedBlock.SetBoolTile(9, 2, true);
                        rightUpCorner = true;
                        break;
                    case 5:
                        rightDownCorner = true;
                        break;
                    case 6:
                        rightDownCorner = true;
                        break;
                    case 7:
                        leftDownCorner = true;
                        break;
                }

        if (leftDownCorner && rightDownCorner && leftUpCorner && rightUpCorner)
        {

        }
        else if (rightUpCorner)
        {
            
        }
    }

    private void GridStrategy (GeneratedBlocks generatedBlock)
    {
        System.Random random = new System.Random();
        bool leftUpCorner = false, rightUpCorner = false, leftDownCorner = false, rightDownCorner = false;
        List<Tuple<bool, Tuple<int,int>>> platforms = new List<Tuple<bool, Tuple<int, int>>>() { Tuple.Create(false, Tuple.Create(2, 2)), Tuple.Create(false, Tuple.Create(2, 3)), Tuple.Create(false, Tuple.Create(7, 3)),
            Tuple.Create(false, Tuple.Create(8, 2)), Tuple.Create(false, Tuple.Create(4, 2)), Tuple.Create(false, Tuple.Create(5, 2)), Tuple.Create(false, Tuple.Create(4, 3)), Tuple.Create(false, Tuple.Create(5, 3)) };

        for (int k = 0; k < 8; k++)
            if (generatedBlock.GetBoolInOut(k))
                switch (k)
                {
                    case 0:
                        leftDownCorner = true;
                        break;
                    case 1:
                        generatedBlock.SetBoolTile(2, 2, true);
                        platforms[0] = Tuple.Create(true, platforms[0].Item2);
                        leftUpCorner = true;
                        break;
                    case 2:
                        generatedBlock.SetBoolTile(2, 3, true);
                        platforms[1] = Tuple.Create(true, platforms[1].Item2);
                        leftUpCorner = true;
                        break;
                    case 3:
                        generatedBlock.SetBoolTile(7, 3, true);
                        platforms[2] = Tuple.Create(true, platforms[2].Item2);
                        rightUpCorner = true;
                        break;
                    case 4:
                        generatedBlock.SetBoolTile(8, 2, true);
                        platforms[3] = Tuple.Create(true, platforms[3].Item2);
                        rightUpCorner = true;
                        break;
                    case 5:
                        rightDownCorner = true;
                        break;
                    case 6:
                        rightDownCorner = true;
                        break;
                    case 7:
                        leftDownCorner = true;
                        break;
                }

        int countPlatform = random.Next(0, 4);

        for (int i = 0; i < countPlatform; i++)
        {
            int idPlat = random.Next(0, 8);

            platforms[idPlat] = Tuple.Create(true, platforms[idPlat].Item2);

            generatedBlock.SetBoolTile(platforms[idPlat].Item2.Item1, platforms[idPlat].Item2.Item2, true);
        }



        if (leftDownCorner && rightDownCorner && leftUpCorner && rightUpCorner)
        {

        }



        else if (rightDownCorner)
        {
            
        }



        else if (leftUpCorner)
        {
            if (platforms[0].Item1 && platforms[1].Item1)
            {
                generatedBlock.SetBoolTile(platforms[0].Item2.Item1, platforms[0].Item2.Item2 + 1, true);

                if (platforms[4].Item1 || platforms[6].Item1)
                {
                    if (!platforms[5].Item1 && !platforms[7].Item1)
                    {
                        CreatePlatform(platforms, generatedBlock, 5);
                    }
                    else
                    {
                        CreatePlatform(platforms, generatedBlock, 3);

                        CreatePlatform(platforms, generatedBlock, 2);
                    }
                }
            }
        }



        else if (rightUpCorner)
        {
            if (!platforms[3].Item1)
            {
                CreatePlatform(platforms, generatedBlock, 3);
            }
        }



        else
        {
            
        }
    }

    public void CreatePlatform(List<Tuple<bool, Tuple<int, int>>> platforms, GeneratedBlocks generatedBlock, int id)
    {
        platforms[id] = Tuple.Create(true, platforms[id].Item2);
        generatedBlock.SetBoolTile(platforms[id].Item2.Item1, platforms[id].Item2.Item2, true);
    }

    private void ClimbWallStrategy(GeneratedBlocks generatedBlock)
    {

    }

    private void DrawMap(GeneratedBlocks[,] generatedBlocks)
    {
        int wT = 0, hT = 0, wB = 0, hB = 0;
        
        float posX = startPositionX, posY = startPositionY;
        int counterBlockX = 0, counterBlockY = 0;

        for (int j = 0; j < countBlocksY * hightBlocks; j++)
        {
            hT = j % hightBlocks;

            for (int i = 0; i < countBlocksX * wightBlocks; i++)
            {
                wT = i % wightBlocks;

                try
                {
                    if (generatedBlocks[wB, hB].GetBoolTile(wT, hT))
                    {
                        Transform clone = Instantiate(tileGround.transform, new Vector3(posX, posY, 5), Quaternion.identity);
                        clone.SetParent(this.transform);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}\nНомер блока ({wB};{hB}) \t Номер тайла ({wT};{hT}) \nCounterX = {counterBlockX}   CounterY = {counterBlockY}");
                }

                posX += hX;

                counterBlockX++;

                if (counterBlockX == wightBlock)
                {
                    wB++;
                    counterBlockX = 0;
                }
            }

            wB = 0;
            posX = startPositionX;

            posY += hY;

            counterBlockY++;

            if (counterBlockY == hightBlock)
            {
                hB++;
                counterBlockY = 0;
            }
        }
    }

    //private void Generate()
    //{
    //    System.Random random = new System.Random();

    //    int maxCount = blocks.block.Count;

    //    int countBlocksX = wight / 2,
    //        countBlocksY = hight / 2;

    //    for (float i = startPointX; i < countBlocksX; i += wightBlock)
    //        for (float j = startPointY; j < countBlocksY; j += hightBlock)
    //        {
    //            int idBlock = random.Next(0, maxCount);

    //            Instantiate(blocks.block[idBlock].transform, new Vector3(i, j, 5), Quaternion.identity);
    //        }
    //}

    private void UpdateSprite()
    {
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("ground");

        //////foreach (GameObject sprite in sprites)
        //////{
        //////    int id = 0;

        //////    for (float i = sprite.transform.position.x - hX; i <= sprite.transform.position.x + hX; i += hX)
        //////        for (float j = sprite.transform.position.y - hY; j <= sprite.transform.position.y + hY; j += hY)
        //////            if (!(i == sprite.transform.position.x && j == sprite.transform.position.y))
        //////            {
        //////                Vector3 center = new Vector3(i, j, 5f);

        //////                fillSprite[id] = SetBoolGround(center);

        //////                id++;
        //////            }

        //////    spriteRenderer = sprite.gameObject.GetComponent<SpriteRenderer>();

        //////    SpriteType1(spriteRenderer);
        //////}

        for (int i = 0; i < sprites.Length; i++)
        {
            Vector3 center = new Vector3(sprites[i].transform.position.x, sprites[i].transform.position.y + hY, 5f);
            top = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x + hX, sprites[i].transform.position.y, 5f);
            right = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x, sprites[i].transform.position.y - hY, 5f);
            bottom = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x - hX, sprites[i].transform.position.y, 5f);
            left = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x + hX, sprites[i].transform.position.y + hY, 5f);
            topRight = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x + hX, sprites[i].transform.position.y - hY, 5f);
            bottomRight = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x - hX, sprites[i].transform.position.y - hY, 5f);
            bottomLeft = SetBoolGround(center);

            center = new Vector3(sprites[i].transform.position.x - hX, sprites[i].transform.position.y + hY, 5f);
            topLeft = SetBoolGround(center);

            spriteRenderer = sprites[i].gameObject.GetComponent<SpriteRenderer>();

            SpriteType(spriteRenderer);
        }
    }

    private bool SetBoolGround(Vector3 center)
    {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(center);

        for (int j = 0; j < hitColliders.Length; j++)
            if (hitColliders[j].gameObject.tag == "ground")
            {
                return true;
            }

        return false;
    }

    private void SpriteType(SpriteRenderer sprite)
    {
        System.Random random = new System.Random();
        int id;

        //Четверной уголок
        if (top && left && right && bottom && !topLeft && !topRight && !bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopDblBottomDbl;

        //Тройные уголки
        else if (top && left && right && bottom && !topLeft && !topRight && bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopDblBottomRight;

        else if (top && left && right && bottom && !topLeft && !topRight && !bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopDblBottomLeft;

        else if (top && left && right && bottom && topLeft && !topRight && !bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopRightBottomDbl;

        else if (top && left && right && bottom && !topLeft && topRight && !bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopLeftBottomDbl;

        //Двойные уголки
        //По вертикали
        else if (top && left && right && bottom && !topLeft && topRight && !bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopBottomLeft;

        else if (top && left && right && bottom && topLeft && !topRight && bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopBottomRight;
        //По горизонтали
        else if (top && left && right && bottom && !topLeft && !topRight && bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopDbl;

        else if (top && left && right && bottom && topLeft && topRight && !bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerBottomDbl;
        //По диагонали
        else if (top && left && right && bottom && topLeft && !topRight && !bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopRightBottomLeft;

        else if (top && left && right && bottom && !topLeft && topRight && bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerTopLeftBottomRight;

        //Одинарные уголки
        else if (top && left && right && bottom && !topLeft && topRight && bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopLeft;

        else if (top && left && right && bottom && topLeft && !topRight && bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerTopRight;

        else if (top && left && right && bottom && topLeft && topRight && bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.cornerBottomRight;

        else if (top && left && right && bottom && topLeft && topRight && !bottomLeft && bottomRight)
            sprite.sprite = greenMaze.cornerBottomLeft;

        //Уголки с верхом и низом
        else if (!top && left && right && bottom && !bottomLeft && bottomRight)
            sprite.sprite = greenMaze.topCornerLeft;

        else if (!top && left && right && bottom && bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.topCornerRight;

        else if (!top && left && right && bottom && !bottomLeft && !bottomRight)
            sprite.sprite = greenMaze.topCornerLeftRight;

        else if (top && left && right && !bottom && !topLeft && topRight)
            sprite.sprite = greenMaze.bottomCornerLeft;

        else if (top && left && right && !bottom && topLeft && !topRight)
            sprite.sprite = greenMaze.bottomCornerRight;

        else if (top && left && right && !bottom && !topLeft && !topRight)
            sprite.sprite = greenMaze.bottomCornerLeftRight;

        //Уголки с боками
        else if (top && !left && right && bottom && topRight && !bottomRight)
            sprite.sprite = greenMaze.leftCornerBottom;

        else if (top && !left && right && bottom && !topRight && bottomRight)
            sprite.sprite = greenMaze.leftCornerTop;

        else if (top && !left && right && bottom && !topRight && !bottomRight)
            sprite.sprite = greenMaze.leftCornerTopBottom;

        else if (top && left && !right && bottom && topLeft && !bottomLeft)
            sprite.sprite = greenMaze.rightCornerBottom;

        else if (top && left && !right && bottom && !topLeft && bottomLeft)
            sprite.sprite = greenMaze.rightCornerTop;

        else if (top && left && !right && bottom && !topLeft && !bottomLeft)
            sprite.sprite = greenMaze.rightCornerTopBottom;

        //Рамка
        else if (!top && !left && right && bottom && !bottomRight)
            sprite.sprite = greenMaze.topLeftCornerRight;

        else if (!top && left && !right && bottom && !bottomLeft)
            sprite.sprite = greenMaze.topRightCornerLeft;

        else if (top && !left && right && !bottom && !topRight)
            sprite.sprite = greenMaze.bottomLeftCornerRight;

        else if (top && left && !right && !bottom && !topLeft)
            sprite.sprite = greenMaze.bottomRightCornerLeft;

        //Одиночки
        else if (!top && !left && right && !bottom)
            sprite.sprite = greenMaze.oneLeft;
        else if (!top && left && !right && !bottom)
            sprite.sprite = greenMaze.oneRight;
        else if (!top && left && right && !bottom)
            sprite.sprite = greenMaze.oneCenter;
        else if (!top && !left && !right && bottom)
            sprite.sprite = greenMaze.onlyTop;
        else if (top && !left && !right && bottom)
            sprite.sprite = greenMaze.onlyCenter;
        else if (top && !left && !right && !bottom)
            sprite.sprite = greenMaze.onlyBottom;

        //Единственный
        else if (!top && !left && !right && !bottom)
            sprite.sprite = greenMaze.onlyOne;

        //Обычные
        //Угловые
        else if (!top && !left && right && bottom)
            sprite.sprite = greenMaze.leftTop;

        else if (!top && left && !right && bottom)
            sprite.sprite = greenMaze.rightTop;

        else if (top && !left && right && !bottom)
            sprite.sprite = greenMaze.leftBottom;

        else if (top && left && !right && !bottom)
            sprite.sprite = greenMaze.rightBottom;

        //Пограничные
        else if (top && !left && right && bottom)
        {
            id = random.Next(1, 3);
            if (id == 1)
                sprite.sprite = greenMaze.left1;
            else sprite.sprite = greenMaze.left2;
        }

        else if (top && left && !right && bottom)
        {
            id = random.Next(1, 3);
            if (id == 1)
                sprite.sprite = greenMaze.right1;
            else sprite.sprite = greenMaze.right2;
        }

        else if (!top && right && left && bottom)
        {
            id = random.Next(1, 3);
            if (id == 1)
                sprite.sprite = greenMaze.top1;
            else sprite.sprite = greenMaze.top2;
        }

        else if (top && right && left && !bottom)
        {
            id = random.Next(1, 3);
            if (id == 1)
                sprite.sprite = greenMaze.bottom1;
            else sprite.sprite = greenMaze.bottom2;
        }

        else sprite.sprite = greenMaze.center;
    }

    //private void SpriteType1(SpriteRenderer sprite)
    //{
    //    System.Random random = new System.Random();
    //    int id;

    //    //Двойные уголки
    //    if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && !fillSprite[5] && fillSprite[7] && !fillSprite[0] && fillSprite[2])
    //        sprite.sprite = greenMaze.cornerTopBottomLeft;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && fillSprite[5] && !fillSprite[7] && fillSprite[0] && !fillSprite[2])
    //        sprite.sprite = greenMaze.cornerTopBottomRight;

    //    //Одинарные уголки
    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && !fillSprite[5] && fillSprite[7] && fillSprite[0] && fillSprite[2])
    //        sprite.sprite = greenMaze.cornerTopLeft;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && fillSprite[5] && !fillSprite[7] && fillSprite[0] && fillSprite[2])
    //        sprite.sprite = greenMaze.cornerTopRight;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && fillSprite[5] && fillSprite[7] && fillSprite[0] && !fillSprite[2])
    //        sprite.sprite = greenMaze.cornerBottomRight;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && fillSprite[5] && fillSprite[7] && !fillSprite[0] && fillSprite[2])
    //        sprite.sprite = greenMaze.cornerBottomLeft;

    //    //Уголки с верхом и низом
    //    else if (!fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && !fillSprite[0] && fillSprite[2])
    //        sprite.sprite = greenMaze.topCornerLeft;

    //    else if (!fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1] && fillSprite[0] && !fillSprite[2])
    //        sprite.sprite = greenMaze.topCornerRight;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && !fillSprite[1] && !fillSprite[5] && fillSprite[7])
    //        sprite.sprite = greenMaze.bottomCornerLeft;

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && !fillSprite[1] && fillSprite[5] && !fillSprite[7])
    //        sprite.sprite = greenMaze.bottomCornerRight;

    //    //Одиночки
    //    else if (!fillSprite[6] && !fillSprite[3] && fillSprite[4] && !fillSprite[1])
    //        sprite.sprite = greenMaze.oneLeft;
    //    else if (!fillSprite[6] && fillSprite[3] && !fillSprite[4] && !fillSprite[1])
    //        sprite.sprite = greenMaze.oneRight;
    //    else if (!fillSprite[6] && fillSprite[3] && fillSprite[4] && !fillSprite[1])
    //        sprite.sprite = greenMaze.oneCenter;

    //    //Обычные
    //    //Угловые
    //    else if (!fillSprite[6] && !fillSprite[3] && fillSprite[4] && fillSprite[1])
    //        sprite.sprite = greenMaze.leftTop;

    //    else if (!fillSprite[6] && fillSprite[3] && !fillSprite[4] && fillSprite[1])
    //        sprite.sprite = greenMaze.rightTop;

    //    else if (fillSprite[6] && !fillSprite[3] && fillSprite[4] && !fillSprite[1])
    //        sprite.sprite = greenMaze.leftBottom;

    //    else if (fillSprite[6] && fillSprite[3] && !fillSprite[4] && !fillSprite[1])
    //        sprite.sprite = greenMaze.rightBottom;

    //    //Пограничные
    //    else if (fillSprite[6] && !fillSprite[3] && fillSprite[4] && fillSprite[1])
    //    {
    //        id = random.Next(1, 3);
    //        if (id == 1)
    //            sprite.sprite = greenMaze.left1;
    //        else sprite.sprite = greenMaze.left2;
    //    }

    //    else if (fillSprite[6] && fillSprite[3] && !fillSprite[4] && fillSprite[1])
    //    {
    //        id = random.Next(1, 3);
    //        if (id == 1)
    //            sprite.sprite = greenMaze.right1;
    //        else sprite.sprite = greenMaze.right2;
    //    }

    //    else if (!fillSprite[6] && fillSprite[3] && fillSprite[4] && fillSprite[1])
    //    {
    //        id = random.Next(1, 3);
    //        if (id == 1)
    //            sprite.sprite = greenMaze.top1;
    //        else sprite.sprite = greenMaze.top2;
    //    }

    //    else if (fillSprite[6] && fillSprite[3] && fillSprite[4] && !fillSprite[1])
    //    {
    //        id = random.Next(1, 3);
    //        if (id == 1)
    //            sprite.sprite = greenMaze.bottom1;
    //        else sprite.sprite = greenMaze.bottom2;
    //    }

    //    else sprite.sprite = greenMaze.center;
    //}

    private void SpawnObject()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

        System.Random rand = new System.Random();
        int id;

        for (int i = 0; i < spawners.Length; i++)
        {
            id = rand.Next(0, gameObjects.Count);

            Transform clone = (Transform) Instantiate(gameObjects[id].transform, new Vector3(spawners[i].transform.position.x, spawners[i].transform.position.y, 5), Quaternion.identity);
            clone.SetParent(transform);

            if (i % 10 == 0)
            {
                clone = (Transform)Instantiate(enemy, new Vector3(spawners[i].transform.position.x, spawners[i].transform.position.y, 5), Quaternion.identity);
                clone.SetParent(transform);
            }
        }
    }
}
