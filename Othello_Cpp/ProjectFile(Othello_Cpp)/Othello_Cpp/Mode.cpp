#define _WINSOCK_DEPRECATED_NO_WARNINGS

#define BUFFER_LEN 5

#include "Mode.h"
#include <iostream>
#include <string>

bool StrToInt(std::string string, int& num, int maxDigits);

Mode::Mode()
{
	type = cpuMode;
}

Mode::Mode(ModeType type)
{
	this->type = type;
}

void Mode::SetModeType(ModeType type)
{
	this->type = type;
}

SOCKET Mode::ListenConnect(std::string portInput)// サーバー側で通信を待ち、通信を受け付ける
{
	int portNum = 0;
	if (!StrToInt(portInput, portNum, 5))
	{
		std::cout << "不正な値です。\n";
		return -1;
	}

	WSADATA wsaData;

	int result = WSAStartup(MAKEWORD(1, 1), &wsaData);
	if (result)
	{
		std::cout << "WSAStartupの失敗。\n";
		return -1;
	}
	std::cout << "WSAStartupの成功。\n";

	SOCKET listenSoc = socket(AF_INET, SOCK_STREAM, 0);
	if (listenSoc < 0)
	{
		std::cout << "ソケットオープンエラー\n";
		WSACleanup();
		return -1;
	}
	std::cout << "リスンソケットオープン完了。\n";

	SOCKADDR_IN saddr;
	ZeroMemory(&saddr, sizeof(SOCKADDR_IN));
	saddr.sin_family = AF_INET;
	saddr.sin_port = htons(portNum);
	saddr.sin_addr.s_addr = INADDR_ANY;
	if (bind(listenSoc, (struct sockaddr*)&saddr, sizeof(saddr)) == SOCKET_ERROR)
	{
		std::cout << "bindのエラー\n";
		closesocket(listenSoc);
		WSACleanup();
		return -1;
	}
	else
		std::cout << "bind成功\n";

	if (listen(listenSoc, 0) == SOCKET_ERROR)
	{
		std::cout << "listen error.\n";
		closesocket(listenSoc);
		WSACleanup();
		return -1;
	}
	else
		std::cout << "listen成功\n";

	std::cout << "-----------------------------------------------------------------------------\n";
	//std::cout << "このプロジェクトと同名のフォルダ(.slnファイルが入っているフォルダ)を開き、\n";
	//std::cout << "「x64」->「Debug」フォルダ内にある「ProgramWorkShop2_Assignment.exe」を起動。\n";
	std::cout << "このプロジェクトの「実行ファイル」フォルダを開き、\n";
	std::cout << "フォルダ内にある「Othello_Cpp.exe」を起動。\n";
	std::cout << "クライアントモードを選択し、IPアドレスとポート番号を入力して接続してください。\n";
	std::cout << "-----------------------------------------------------------------------------\n\n";
	std::cout << "クライアントの接続を待機しています...\n";

	SOCKADDR_IN from;
	int fromlen = sizeof(from);
	SOCKET soc = accept(listenSoc, (SOCKADDR*)&from, &fromlen);
	if (soc == INVALID_SOCKET)
	{
		std::cout << "accept error.\n";
		closesocket(listenSoc);
		WSACleanup();
		return -1;
	}
	else
		std::cout << inet_ntoa(from.sin_addr) << "が接続してきました。\n";

	closesocket(listenSoc);
	std::cout << "リスンソケットクロース完了。\n";

	return soc;
}

SOCKET Mode::Connect(std::string portInput, const char* serverIP)// クライアント側で接続を要求する
{
	int portNum = 0;
	if (!StrToInt(portInput, portNum, 5))
	{
		std::cout << "不正な値です。\n";
		return -1;
	}

	WSADATA wsaData;

	int result = WSAStartup(MAKEWORD(1, 1), &wsaData);
	if (result)
	{
		std::cout << "WSAStartupの失敗。\n";
		return -1;
	}
	std::cout << "WSAStartupの成功。\n";

	SOCKET soc = socket(AF_INET, SOCK_STREAM, 0);
	if (soc < 0)
	{
		std::cout << "ソケットオープンエラー\n";
		WSACleanup();
		return -1;
	}

	HOSTENT* lpHost = gethostbyname(serverIP);
	if (lpHost == NULL)
	{
		unsigned int addr = inet_addr(serverIP);
		lpHost = gethostbyaddr((char*)&addr, 4, AF_INET);
	}
	if (lpHost == NULL)
	{
		std::cout << "gethostbyaddrのエラー\n";
		closesocket(soc);
		WSACleanup();
		return -1;
	}

	SOCKADDR_IN saddr;
	ZeroMemory(&saddr, sizeof(SOCKADDR_IN));
	saddr.sin_family = lpHost->h_addrtype;
	saddr.sin_port = htons(portNum);
	saddr.sin_addr.s_addr = *((u_long*)lpHost->h_addr);
	if (connect(soc, (SOCKADDR*)&saddr, sizeof(saddr)) == SOCKET_ERROR)
	{
		std::cout << "connectのエラー\n";
		closesocket(soc);
		WSACleanup();
		return -1;
	}
	else
		std::cout << "connect成功\n";
	return soc;
}

void Mode::TurnProcessing(Board& board, Player& player, Player& enemy, SOCKET socket)// ソケット通信による対戦時のターン処理
{
	char buffer[BUFFER_LEN];
	int inputX, inputY, rcv;
	bool enteredExit = false;
	bool autoPlace = false;
	bool firstGetExit = false;
	Grid placeGrid;
	if (type == serverMode)
	{
		enemy.SetPlaceableGrids(board.SearchPlaceableGrids(enemy.GetPlayerStoneType()));
		board.ShowBoard(enemy, (Board::playType)type);
		std::cout << "受信待ち\n";
		rcv = recv(socket, buffer, sizeof(buffer) - 1, 0);
		if (rcv == SOCKET_ERROR)
		{
			std::cout << "エラーです。\n";
			firstGetExit = true;
			enteredExit = true;
			return;
		}
		buffer[rcv] = '\0';
		if (strcmp(buffer, "Exit") == 0)
		{
			std::cout << "クライアントが切断\n";
			enteredExit = true;
			return;
		}
		if (buffer[0] == 'p' && buffer[1] == 's')
		{
			enemy.SetHasSkipped(true);
			std::cout << "相手がパスしました。\n";
		}
		else if ((buffer[0] == 'e' && buffer[1] == 'n') || (buffer[0] == 'y' || buffer[0] == 'n'))
		{
			firstGetExit = true;
			enteredExit = true;
		}
		else
		{
			inputX = buffer[0];
			inputY = buffer[1];
			board.PlaceAndFlipStones(board.GetGrid(inputX, inputY), enemy);
			std::cout << "設置：" << inputX << ',' << inputY << '\n';
			board.ShowBoard();
		}
	}
	if (firstGetExit)// クライアントから最初に「Exit」と入力されたとき
	{
		buffer[0] = 'e';
		buffer[1] = 'n';
		buffer[2] = '\0';
		send(socket, buffer, int(strlen(buffer)), 0);

		board.ShowBoard();
		std::cout << "ゲーム終了！\n";
		autoPlace = false;
		board.SetStoneTypeNum();
		std::cout << "白の石（サーバー）:" << board.GetWhiteNum() << "個\n";
		std::cout << "黒の石（クライアント）：" << board.GetBlackNum() << "個\n";
		if (board.GetWhiteNum() > board.GetBlackNum())
		{
			std::cout << "サーバーの勝ちです！\n";
		}
		else if (board.GetBlackNum() > board.GetWhiteNum())
		{
			std::cout << "クライアントの勝ちです！\n";
		}
		else
		{
			std::cout << "引き分けです！\n";
		}
	}
	else
	{
		while (true)
		{
			enteredExit = false;
			switch (type)
			{
			case serverMode:
				player.SetPlaceableGrids(board.SearchPlaceableGrids(player.GetPlayerStoneType()));
				board.ShowBoard(player, (Board::playType)type);
				std::cout << "あなたのターンです。\n";
				if (player.GetPlaceableGrids().size() != 0)
				{
					board.SetStonesByInput(player, enteredExit, placeGrid, autoPlace);
					if (!enteredExit)
					{
						buffer[0] = placeGrid.xPos();
						buffer[1] = placeGrid.yPos();
						buffer[2] = '\0';
					}
				}
				else
				{
					std::cout << "置ける場所が無いので、パスします。\n";
					player.SetHasSkipped(true);
				}
				std::cout << '\n';

				if (enteredExit)
				{
					buffer[0] = 'e';
					buffer[1] = 'n';
					buffer[2] = '\0';
					send(socket, buffer, int(strlen(buffer)), 0);
					break;
				}
				if (player.GetHasSkipped())
				{
					buffer[0] = 'p';
					buffer[1] = 's';
					buffer[2] = '\0';
				}
				send(socket, buffer, int(strlen(buffer)), 0);

				enemy.SetPlaceableGrids(board.SearchPlaceableGrids(enemy.GetPlayerStoneType()));
				board.ShowBoard(enemy, (Board::playType)type);
				std::cout << "受信待ち\n";
				rcv = recv(socket, buffer, sizeof(buffer) - 1, 0);
				if (rcv == SOCKET_ERROR)
				{
					std::cout << "エラーです。\n";
					enteredExit = true;
					break;
				}
				buffer[rcv] = '\0';
				if (strcmp(buffer, "Exit") == 0)
				{
					std::cout << "クライアントが切断\n";
					enteredExit = true;
					break;
				}

				if (buffer[0] == 'p' && buffer[1] == 's')
				{
					enemy.SetHasSkipped(true);
					std::cout << "相手がパスしました。\n";
				}
				else if ((buffer[0] == 'e' && buffer[1] == 'n') || (buffer[0] == 'y' || buffer[0] == 'n'))
				{
					enteredExit = true;
					break;
				}
				else
				{
					inputX = buffer[0];
					inputY = buffer[1];
					board.PlaceAndFlipStones(board.GetGrid(inputX, inputY), enemy);
					std::cout << "設置：" << inputX << ',' << inputY << '\n';
					board.ShowBoard();
				}
				break;

			case clientMode:
				player.SetPlaceableGrids(board.SearchPlaceableGrids(player.GetPlayerStoneType()));
				board.ShowBoard(player, (Board::playType)type);
				std::cout << "あなたのターンです。\n";
				if (player.GetPlaceableGrids().size() != 0)
				{
					board.SetStonesByInput(player, enteredExit, placeGrid, autoPlace);
					if (!enteredExit)
					{
						buffer[0] = placeGrid.xPos();
						buffer[1] = placeGrid.yPos();
						buffer[2] = '\0';
					}
				}
				else
				{
					std::cout << "置ける場所が無いので、パスします。\n";
					player.SetHasSkipped(true);
				}
				std::cout << '\n';

				if (enteredExit)
				{
					buffer[0] = 'e';
					buffer[1] = 'n';
					buffer[2] = '\0';
					send(socket, buffer, int(strlen(buffer)), 0);
					enteredExit = true;
					break;
				}
				if (player.GetHasSkipped())
				{
					buffer[0] = 'p';
					buffer[1] = 's';
					buffer[2] = '\0';
				}
				send(socket, buffer, int(strlen(buffer)), 0);

				enemy.SetPlaceableGrids(board.SearchPlaceableGrids(enemy.GetPlayerStoneType()));
				board.ShowBoard(enemy, (Board::playType)type);
				std::cout << "受信待ち\n";
				rcv = recv(socket, buffer, sizeof(buffer) - 1, 0);
				if (rcv == SOCKET_ERROR)
				{
					std::cout << "エラーです。\n";
					enteredExit = true;
					break;
				}
				buffer[rcv] = '\0';
				if (strcmp(buffer, "Exit") == 0)
				{
					std::cout << "クライアントが切断\n";
					enteredExit = true;
					break;
				}

				if (buffer[0] == 'p' && buffer[1] == 's')
				{
					enemy.SetHasSkipped(true);
					std::cout << "相手がパスしました。\n";
				}
				else if ((buffer[0] == 'e' && buffer[1] == 'n') || (buffer[0] == 'y' || buffer[0] == 'n'))
				{
					enteredExit = true;
					break;
				}
				else
				{
					inputX = buffer[0];
					inputY = buffer[1];
					board.PlaceAndFlipStones(board.GetGrid(inputX, inputY), enemy);
					std::cout << "設置：" << inputX << ',' << inputY << '\n';
					board.ShowBoard();
				}
				break;
			default:
				break;
			}

			if (board.CheckGameOver(player, enemy) || enteredExit)
			{
				buffer[0] = 'e';
				buffer[1] = 'n';
				buffer[2] = '\0';
				send(socket, buffer, int(strlen(buffer)), 0);

				board.ShowBoard();
				std::cout << "ゲーム終了！\n";
				autoPlace = false;
				board.SetStoneTypeNum();
				std::cout << "白の石（サーバー）:" << board.GetWhiteNum() << "個\n";
				std::cout << "黒の石（クライアント）：" << board.GetBlackNum() << "個\n";
				if (board.GetWhiteNum() > board.GetBlackNum())
				{
					std::cout << "サーバーの勝ちです！\n";
				}
				else if (board.GetBlackNum() > board.GetWhiteNum())
				{
					std::cout << "クライアントの勝ちです！\n";
				}
				else
				{
					std::cout << "引き分けです！\n";
				}
				break;
				//enteredExit = false;
				//char retryChar[BUFFER_LEN];
				//bool retry = false;
				//int rcv;
				//while (true)
				//{
				//	std::cout << "\n再戦しますか？Yes：y　No：n\n";
				//	std::cout << "入力：";
				//	std::cin >> retryChar;
				//	if (retryChar[0] == 'y' || retryChar[0] == 'n')
				//	{
				//		switch (type)
				//		{
				//		case serverMode:
				//			if (retryChar[0] == 'n')
				//			{
				//				std::cout << "再戦を拒否します。\n";
				//				retry = false;
				//			}
				//			std::cout << "相手を待っています...\n";
				//			if (buffer[0] != 'y' && buffer[0] != 'n')
				//			{
				//				rcv = recv(socket, buffer, sizeof(buffer) - 1, 0);
				//				if (rcv == SOCKET_ERROR)
				//				{
				//					std::cout << "エラーです。\n";
				//					retry = false;
				//					break;
				//				}
				//			}
				//			buffer[rcv] = '\0';
				//			if (strcmp(buffer, "y") == 0 && retryChar[0] == 'y')
				//			{
				//				retry = true;
				//			}
				//			else if (retryChar[0] == 'y')
				//			{
				//				std::cout << "対戦相手が再戦を拒否しました。\n";
				//				retry = false;
				//			}
				//			send(socket, retryChar, int(strlen(retryChar)), 0);
				//			break;
				//		case clientMode:
				//			if (retryChar[0] == 'n')
				//			{
				//				std::cout << "再戦を拒否します。\n";
				//				retry = false;
				//			}
				//			send(socket, retryChar, int(strlen(retryChar)), 0);
				//			std::cout << "相手を待っています...\n";
				//			if (buffer[0] != 'y' && buffer[0] != 'n')
				//			{
				//				rcv = recv(socket, buffer, sizeof(buffer) - 1, 0);
				//				if (rcv == SOCKET_ERROR)
				//				{
				//					std::cout << "エラーです。\n";
				//					retry = false;
				//					break;
				//				}
				//			}
				//			buffer[rcv] = '\0';
				//			if (strcmp(buffer, "y") == 0 && retryChar[0] == 'y')
				//			{
				//				retry = true;
				//			}
				//			else if (retryChar[0] == 'y')
				//			{
				//				std::cout << "対戦相手が再戦を拒否しました。\n";
				//				retry = false;
				//			}
				//			break;
				//		default:
				//			break;
				//		}
				//		break;
				//	}
				//}
				//if (retry)
				//{
				//	enteredExit = false;
				//	player.SetHasSkipped(false);
				//	enemy.SetHasSkipped(false);
				//	board.ResetBoard();
				//	std::cout << "再戦します。\n";
				//	TurnProcessing(board, player, enemy, socket);
				//	//continue;
				//}
				//else
				//{
				//	break;
				//}
			}
			else
			{
				player.SetHasSkipped(false);
				enemy.SetHasSkipped(false);
			}
		}
	}

	shutdown(socket, SD_BOTH);
	closesocket(socket);
	std::cout << "プログラムを終了します。エンターキーを押してください。\n";// 自動設置モードのとき、試合終了時にウィンドウも閉じられてしまうため
	std::cin.ignore();
	std::string enterInput;
	std::getline(std::cin, enterInput);

	int result = WSACleanup();
	if (result)
	{
		std::cout << "WSACleanupの失敗。\n";
		return;
	}
	std::cout << "WSACleanupの成功。\n";
}

void Mode::TurnProcessing(Board& board, Player& player, Player& enemy)// CPUとの対戦時のターン処理
{
	bool enteredExit = false;
	bool autoPlace = false;
	Grid placeGrid;
	while (true)
	{
		player.SetPlaceableGrids(board.SearchPlaceableGrids(player.GetPlayerStoneType()));
		board.ShowBoard(player);
		std::cout << "あなたのターンです。\n";
		if (player.GetPlaceableGrids().size() != 0)
		{
			board.SetStonesByInput(player, enteredExit, placeGrid, autoPlace);
		}
		else
		{
			std::cout << "置ける場所が無いので、パスします。\n";
			player.SetHasSkipped(true);
		}
		std::cout << '\n';
		if (!enteredExit)
		{
			enemy.SetPlaceableGrids(board.SearchPlaceableGrids(enemy.GetPlayerStoneType()));
			board.ShowBoard(enemy);
			std::cout << "CPUのターンです。\n";

			if (enemy.GetPlaceableGrids().size() != 0)
			{
				Grid player2PlaceGrid = enemy.GetPlaceGridRandomly();
				std::cout << "縦：" << player2PlaceGrid.yPos() << '\n';
				std::cout << "横：" << player2PlaceGrid.xPos() << '\n';
				board.PlaceAndFlipStones(player2PlaceGrid, enemy);
			}
			else
			{
				std::cout << "置ける場所が無いので、パスします。\n";
				enemy.SetHasSkipped(true);
			}
			std::cout << '\n';
		}
		if (board.CheckGameOver(player, enemy) || enteredExit)
		{
			board.ShowBoard();
			std::cout << "ゲーム終了！\n";
			board.SetStoneTypeNum();
			std::cout << "白の石（あなた）:" << board.GetWhiteNum() << "個\n";
			std::cout << "黒の石（CPU）：" << board.GetBlackNum() << "個\n";
			if (board.GetWhiteNum() > board.GetBlackNum())
			{
				std::cout << "あなたの勝ちです！\n";
			}
			else if (board.GetBlackNum() > board.GetWhiteNum())
			{
				std::cout << "CPUの勝ちです！\n";
			}
			else
			{
				std::cout << "引き分けです！\n";
			}
			std::string retryString;
			while (true)
			{
				std::cout << "\n再戦しますか？Yes：y　No：n\n";
				std::cout << "入力：";
				std::cin >> retryString;
				if (retryString == "y" || retryString == "n")
				{
					break;
				}
			}
			if (retryString == "y")
			{
				enteredExit = false;
				autoPlace = false;
				player.SetHasSkipped(false);
				enemy.SetHasSkipped(false);
				board.ResetBoard();
				continue;
			}
			else
			{
				std::cout << "プログラムを終了します。\n";
				break;
			}
		}
		else
		{
			player.SetHasSkipped(false);
			enemy.SetHasSkipped(false);
		}
	}
}

bool StrToInt(std::string string, int& num, int maxDigits)
{
	if (string.size() > maxDigits)
	{
		return false;
	}
	int magnification = 1;
	for (int i = 0; i < string.size(); i++)
	{
		int digitNum = string[string.size() - (i + 1)] - '0';
		if (digitNum < 0 || digitNum > 9)
		{
			return false;
		}
		num += digitNum * magnification;
		magnification *= 10;
	}
	return true;
}