export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
	id: string;
	type: ToastType;
	message: string;
	duration?: number;
}

interface ToastState {
	toasts: Toast[];
}

function createToastStore() {
	let state = $state<ToastState>({
		toasts: []
	});

	let nextId = 0;

	function addToast(type: ToastType, message: string, duration = 5000) {
		const id = `toast-${nextId++}`;
		const toast: Toast = { id, type, message, duration };

		state.toasts = [...state.toasts, toast];

		if (duration > 0) {
			setTimeout(() => {
				removeToast(id);
			}, duration);
		}

		return id;
	}

	function removeToast(id: string) {
		state.toasts = state.toasts.filter((t) => t.id !== id);
	}

	return {
		get toasts() {
			return state.toasts;
		},

		success(message: string, duration?: number) {
			return addToast('success', message, duration);
		},

		error(message: string, duration?: number) {
			return addToast('error', message, duration);
		},

		warning(message: string, duration?: number) {
			return addToast('warning', message, duration);
		},

		info(message: string, duration?: number) {
			return addToast('info', message, duration);
		},

		remove(id: string) {
			removeToast(id);
		},

		clear() {
			state.toasts = [];
		}
	};
}

export const toastStore = createToastStore();
