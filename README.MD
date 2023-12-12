# Relaxator

Relaxator is a Console Application written in C# that calculates your earnings in real-time based on your salary and a regular time interval (delay). The application also converts your earnings to Russian Rubles (RUB) based on the current exchange rate.

## Features
- Real-time earning calculation
- Salary input in USD
- Automatic currency conversion to RUB
- User-friendly display of total earnings
- Graceful shutdown with total earnings display on `Ctrl+C`

## Dependencies
The application uses the [AngleSharp](https://github.com/AngleSharp/AngleSharp) library to parse the web page for the current currency exchange rate.

## Installation
Clone the repository to your local machine.
Make sure to restore the required packages:
```sh
dotnet restore
```
Build the project:
```sh
dotnet build
```

## Usage
Run the application with the following command, replacing `salary` with your salary in USD and `delay` with the delay in seconds for calculation updates:
```sh
dotnet run -- -s <salary> -d <delay>
```
Example:
```sh
dotnet run -- -s 100000 -d 1
```

### Options
- `-s`: Your monthly salary in USD
- `-d`: Delay (interval) in seconds for earnings calculation refresh

### Console Output
During execution, the application displays your earnings in real-time:
```
You've earned: 100💲 (9047₽)
```
X is your real-time earning in USD, and Y is the converted amount in RUB.

To stop the application and see your total earnings, use:
```sh
Ctrl+C
```
Upon stopping, the application will clear the console and show you the total amount made during its run.

## Currency Rate Fetching
The application fetches the exchange rate from [Profinance.ru](https://www.profinance.ru/currency_usd.asp). If for any reason the website is down or the layout changes, the currency rate fallbacks to 0, and the conversion will not be shown.

## Contributions
Contributions are welcome. Feel free to open a pull request or an issue in the GitHub repository.

## License
This project is open source and available under the [MIT License](LICENSE).