model {
    user = person "Player" {
        description "A person playing the game"
    }

    uttt = softwareSystem "uttt - Ultimate Tic Tac Toe" {
        description "uttt - Ultimate Tic Tac Toe"

        webapp = container "Static Web App" {
            technology "Maui .NET App"

            view = component "View" {
                description "Renders the UI"
                technology "Blazor Web Views"
            }

            viewModel = component "View Model" {
                description "Handles UI Events"
            }

            model = component "Model" {
                description "Contains the domain / game logic"
            }
        }

        user -> view "Uses"
        view -> viewModel "recieves events"
        model -> viewModel "updates UI based on game logic"
        viewModel -> model "updates game logic based on user feedback"
    }
}