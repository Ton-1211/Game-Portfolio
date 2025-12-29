#include "Player.h"

Player::Player()
{
	playerStoneType = Grid::none;
	placeableGrids.clear();
	hasSkipped = false;
	engine = std::default_random_engine(seedGen());
}

Player::Player(Grid::stoneType type)
{
	playerStoneType = type;
	placeableGrids.clear();
	hasSkipped = false;
	engine = std::default_random_engine(seedGen());
}

bool Player::TryGetPlaceGrid(int x, int y, Grid& placeGrid)
{
	for (int i = 0; i < placeableGrids.size(); i++)
	{
		if (placeableGrids[i].xPos() == x && placeableGrids[i].yPos() == y)
		{
			placeGrid = placeableGrids[i];
			return true;
		}
	}
	return false;
}

Grid Player::GetPlaceGridRandomly()// 設置可能な場所からランダムで設置可能な場所を返す
{
	std::uniform_int_distribution<> dist(0, placeableGrids.size() - 1);
	int randomGrid = dist(engine);

	return placeableGrids[randomGrid];
}

void Player::SetPlaceableGrids(std::vector<Grid> grids)
{
	placeableGrids = grids;
}

void Player::SetHasSkipped(bool flag)
{
	hasSkipped = flag;
}