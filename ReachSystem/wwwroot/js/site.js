document.addEventListener("DOMContentLoaded", function () {

    const profile = document.querySelector(".profile-preview");
    const profileButton = document.querySelector(".profile-avatar");

    if (!profile || !profileButton) {
        return;
    }

    profileButton.addEventListener("click", function (event) {

        event.stopPropagation();

        profile.classList.toggle("is-open");

    });


    /*
       Impede que clicar dentro do mini perfil
       feche ele imediatamente.
    */

    const popover = profile.querySelector(".profile-popover");

    if (popover) {

        popover.addEventListener("click", function (event) {
            event.stopPropagation();
        });

    }


    /*
       Clicar fora do perfil fecha o mini perfil.
    */

    document.addEventListener("click", function () {

        profile.classList.remove("is-open");

    });

});