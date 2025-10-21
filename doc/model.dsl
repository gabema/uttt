model {
    user = person "Player" {
        description "A person playing the game"
    }

    uttt = softwareSystem "uttt - Ultimate Tic Tac Toe" {
        description "uttt - Ultimate Tic Tac Toe"

        webapp = container "Static Web App" {
            technology ".NET 8 Blazor App"

            app = component "uttt.app" {
                description "Ultimate Tic Tac Toe App"
                technology "Blazor"
            }

            game = component "uttt.game" {
                description "Contains the domain / game logic"
            }
        }

        user -> app "Uses"
        app -> game "translates UI events"
        game -> app "Updates game based on state"
    }
}