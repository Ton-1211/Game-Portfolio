#include "Grid.h"

Grid::Grid()
{
	type = none;
	x = 0;
	y = 0;
	connectedStones.clear();
}

Grid::Grid(stoneType type, int x, int y)
{
	this->type = type;
	this->x = x;
	this->y = y;
	connectedStones.clear();
}

void Grid::SetStoneType(stoneType type)
{
	this->type = type;
}

void Grid::SetConnectedStones(Grid stone)
{
	this->connectedStones.emplace_back(stone);
}

void Grid::SetConnectedStones(std::vector<Grid> stones)
{
	this->connectedStones = stones;
}