#include "Grid.h"
#include "Player.h"
#include <vector>

#ifndef BOARD
#define BOARD
class Board
{
public:
	enum direction
	{
		up, down, left, right, upperLeft, upperRight, lowerLeft, lowerRight, max
	};
	enum playType
	{
		vsCPUMode = 0,
		serverMode = 1,
		clientMode = 2
	};
private:
	int width;
	int height;
	int WhiteNum;
	int BlackNum;
	std::vector<std::vector<Grid>> grids;

public:
	Board();
	Board(int width, int height);
	int GetWidth() const{ return width; }// ”Õ–Ê‚Ì‰¡‚ÌL‚³‚ğ•Ô‚·
	int GetHeight() const { return height; }// ”Õ–Ê‚Ìc‚ÌL‚³‚ğ•Ô‚·
	int GetWhiteNum() const { return WhiteNum; }// ”Õ–Êã‚Ì”’‚ÌÎ‚Ì”‚ğ•Ô‚·
	int GetBlackNum() const { return BlackNum; }// ”Õ–Êã‚Ì•‚ÌÎ‚Ì”‚ğ•Ô‚·
	void SetWhiteNum(int Num);
	void SetBlackNum(int Num);
	bool TryGetGrid(Grid grid, direction dir, Grid& outputGrid);
	std::vector<Grid> const SearchFlippableGrids(Grid gridBase, direction dir, Grid::stoneType type);
	std::vector<Grid> const SearchPlaceableGrids(Grid::stoneType type);
	void PlaceAndFlipStones(Grid grid, Player& player);
	void SetStonesByInput(Player& player, bool& enteredExit, Grid& outPutGrid, bool& autoPlace);
	void const ShowBoard();
	void const ShowBoard(Player& player);
	void const ShowBoard(Player& player, playType type);
	bool const CheckGameOver(Player& playerOne, Player& playerTwo);
	void const SetStoneTypeNum();
	void ResetBoard();
	Grid GetGrid(int x, int y);
};

#endif