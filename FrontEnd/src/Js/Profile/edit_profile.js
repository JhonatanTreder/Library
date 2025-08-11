const buttons = document.querySelectorAll('.box-button');
let editing = false;

buttons.forEach((button) => {
    button.addEventListener('click', () => {
        if (editing && button.textContent !== 'Salvar') return;

        const infoItem = button.parentElement;
        const contentSpan = infoItem.querySelector('.info-content');

        if (!editing) {
            editing = true;

            // Guarda o valor original para restaurar caso clique em "Cancelar"
            const originalText = contentSpan.textContent;

            // Desativa outros botões
            buttons.forEach((btn) => {
                if (btn !== button) {
                    btn.disabled = true;
                    btn.classList.remove('box-button');
                    btn.classList.add('box-button-unavailable');
                }
            });

            // Cria o input e define valor
            const input = document.createElement('input');
            input.type = 'text';
            input.value = originalText;
            input.classList.add('info-input');

            // Substitui o span pelo input
            contentSpan.replaceWith(input);
            input.focus();

            // Altera o botão para "Salvar"
            button.textContent = 'Salvar';

            // Cria o botão "Cancelar"
            const cancelButton = document.createElement('button');
            cancelButton.textContent = 'Cancelar';
            cancelButton.classList.add('cancel-button');
            infoItem.appendChild(cancelButton);

            // Evento do botão "Cancelar"
            cancelButton.addEventListener('click', () => {
                editing = false;

                // Reativa os outros botões
                buttons.forEach((btn) => {
                    btn.disabled = false;
                    btn.classList.remove('box-button-unavailable');
                    btn.classList.add('box-button');
                });

                // Cria de volta o span com o texto original
                const restoredSpan = document.createElement('span');
                restoredSpan.classList.add('info-content');
                restoredSpan.textContent = originalText;

                // Substitui o input pelo span original
                input.replaceWith(restoredSpan);

                // Restaura o botão
                button.textContent = 'Editar';
                cancelButton.remove();
            });

        } else {
            // Se clicou em "Salvar"
            const input = infoItem.querySelector('input');
            const newSpan = document.createElement('span');
            newSpan.classList.add('info-content');
            newSpan.textContent = input.value;

            input.replaceWith(newSpan);
            button.textContent = 'Editar';
            editing = false;

            buttons.forEach((btn) => {
                btn.disabled = false;
                btn.classList.remove('box-button-unavailable');
                btn.classList.add('box-button');
            });

            const cancelBtn = infoItem.querySelector('.cancel-button');
            if (cancelBtn) cancelBtn.remove();
        }
    });
});
