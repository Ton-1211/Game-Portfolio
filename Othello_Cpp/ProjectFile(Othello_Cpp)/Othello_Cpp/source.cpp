#include "Board.h"
#include "Mode.h"
#include <iostream>
#include <WinSock2.h>
#include <string>

using namespace std;

enum PlayType
{
	cpuFightMode = 0,
	serverMode = 1,
	clientMode = 2,
	maxNum = 3
};

int main()
{
	int modeDigit = 1;// モード選択時の桁数
	PlayType playType;
	Player player1(Grid::white), player2(Grid::black);
	Mode player1Mode, player2Mode;
	while (true)
	{
		cout << "モードを選択してください： 0:CPU戦 1:ネットワーク対戦（サーバー） 2:ネットワーク対戦（クライアント）";
		string typeInput;
		cin >> typeInput;
		std::cin.ignore();
		if (typeInput.size() > modeDigit)
		{
			cout << modeDigit << "ケタにしてください。\n";
			continue;
		}
		int typeNum = typeInput[0] - '0';
		if (typeNum < 0 || typeNum >(int)maxNum)
		{
			cout << "値が不正です。\n";
			continue;
		}
		playType = (PlayType)typeNum;
		break;
	}

	SOCKET socket;
	if (playType == cpuFightMode)
	{
		player1Mode.SetModeType(Mode::vsCPUMode);
		player2Mode.SetModeType(Mode::cpuMode);
	}
	if (playType == serverMode || playType == clientMode)
	{
		player1Mode.SetModeType(Mode::serverMode);
		player2Mode.SetModeType(Mode::clientMode);
	}
	char serverIP[40];
	if (playType == serverMode || playType == clientMode)
	{
		if (playType == clientMode)
		{
			cout << "サーバーのIPアドレスを入力(デフォルトは127.0.0.1)：";
			string serverIPInput;
			getline(cin, serverIPInput);
			if (serverIPInput.empty() || serverIPInput.size() >= 40)
			{
				serverIPInput = "127.0.0.1";
			}

			cout << "IPアドレス：" << serverIPInput << '\n';
			serverIPInput.copy(serverIP, serverIPInput.size() + 1);
			serverIP[serverIPInput.size()] = '\0';
		}

		cout << "ポート番号を入力(5ケタ)(デフォルトは55555)：";
		string portInput, defaultPortInput = "55555";
		getline(cin, portInput);
		if (portInput.empty() || portInput.size() > 5)
		{
			portInput = defaultPortInput;
		}
		cout << "------------------------------\n";
		cout << "ポート番号を" << portInput << "に設定します。\n";
		cout << "------------------------------\n";

		if (playType == serverMode)// サーバーのとき
		{
			socket = player1Mode.ListenConnect(portInput);
		}
		else// クライアントのとき
		{
			socket = player2Mode.Connect(portInput, serverIP);
		}
	}

	Board board(8, 8);

	if (playType == serverMode)
	{
		player1Mode.TurnProcessing(board, player1, player2, socket);
	}
	else if (playType == clientMode)
	{
		player2Mode.TurnProcessing(board, player2, player1, socket);
	}
	else
	{
		player1Mode.TurnProcessing(board, player1, player2);
	}
}