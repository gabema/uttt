workspace "uttt - Ultimate Tic Tac Toe" "Architecture documentation for My Project" {

    !include model.dsl
    !adrs ./adr adrtools

    views {
        systemContext uttt {
            include *
            autolayout lr
        }

        container uttt {
            include *
            autolayout lr
        }

        component webapp {
            include *
            autolayout lr
        }

        theme default
    }
}
