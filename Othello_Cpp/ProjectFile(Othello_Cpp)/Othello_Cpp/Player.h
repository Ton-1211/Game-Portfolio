#include "Grid.h"
#include <vector>
#include <random>
#include<WinSock2.h>

#ifndef PLAYER
#define PLAYER
class Player
{
private:
	Grid::stoneType playerStoneType;
	std::vector<Grid> placeableGrids;
	bool hasSkipped;
	std::random_device seedGen;
	std::default_random_engine engine;

public:
	Player();
	Player(Grid::stoneType type);
	Grid::stoneType GetPlayerStoneType() const { return playerStoneType; }// プレイヤーの石の色を返す
	std::vector<Grid> GetPlaceableGrids() const { return placeableGrids; }// プレイヤーの設置可能な場所の一覧を返す
	bool GetHasSkipped() const { return hasSkipped; }// スキップしたかどうかを返す
	bool TryGetPlaceGrid(int x, int y, Grid& grid);
	Grid GetPlaceGridRandomly();
	void SetPlaceableGrids(std::vector<Grid> grids);
	void SetHasSkipped(bool flag);
};
#endif // !PLAYER

