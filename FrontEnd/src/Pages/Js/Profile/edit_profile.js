const buttons = document.querySelectorAll('.box-button');
let editing = false;

buttons.forEach((button) => {
    button.addEventListener('click', () => {
        if (editing && button.textContent !== 'Salvar') return;

        const infoItem = button.parentElement;
        const contentSpan = infoItem.querySelector('.info-content');

        if (!editing) {
            editing = true;

            const originalText = contentSpan.textContent;

            buttons.forEach((btn) => {
                if (btn !== button) {
                    btn.disabled = true;
                    btn.classList.remove('box-button');
                    btn.classList.add('box-button-unavailable');
                }
            });

            const input = document.createElement('input');
            input.type = 'text';
            input.value = originalText;
            input.classList.add('info-input');

            contentSpan.replaceWith(input);
            input.focus();

            button.textContent = 'Salvar';

            const cancelButton = document.createElement('button');
            cancelButton.textContent = 'Cancelar';
            cancelButton.classList.add('cancel-button');
            infoItem.appendChild(cancelButton);

            cancelButton.addEventListener('click', () => {
                editing = false;

                buttons.forEach((btn) => {
                    btn.disabled = false;
                    btn.classList.remove('box-button-unavailable');
                    btn.classList.add('box-button');
                });

                const restoredSpan = document.createElement('span');
                restoredSpan.classList.add('info-content');
                restoredSpan.textContent = originalText;

                input.replaceWith(restoredSpan);

                button.textContent = 'Editar';
                cancelButton.remove();
            });

        } else {

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
