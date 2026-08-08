model {
    user = person "Player" {
        description "A person playing the game"
    }

    uttt = softwareSystem "uttt - Ultimate Tic Tac Toe" {
        description "uttt - Ultimate Tic Tac Toe"

        webapp = container "Static Web App" {
            technology "Rust / WebAssembly (Leptos), built with Trunk"

            view = component "uttt-web (View)" {
                description "Leptos frontend. Renders from the engine projection, forwards clicks, and owns only the capture animation. Holds no game logic and no authoritative state."
                technology "Rust, Leptos, WASM"
            }

            engine = component "uttt-core (Engine)" {
                description "Pure game logic: board model, win/draw detection, the move rule and turn state, and the BoardView projection the UI renders from."
                technology "Rust (no UI/WASM dependencies)"
            }
        }

        user -> view "Plays via clicks"
        view -> engine "Forwards moves (play); reads the projection (view)"
        engine -> view "Provides BoardView projection + MoveOutcome"
    }
}
