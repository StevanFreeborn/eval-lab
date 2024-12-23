export function convertToTitleCase(str: string) {
  return str.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
}

export function createThrottledHandler<T>(
  handlerFn: (value: T) => Promise<void>,
  delay: number = 1000,
): (value: T) => Promise<void> {
  let isWaiting = false;
  let pendingValue: T | null = null;

  async function processPending() {
    if (pendingValue === null) {
      isWaiting = false;
      return;
    }

    const valueToProcess = pendingValue;
    pendingValue = null;

    await handlerFn(valueToProcess);
    setTimeout(processPending, delay);
  }

  return async function (value: T): Promise<void> {
    if (isWaiting) {
      pendingValue = value;
      return;
    }

    await handlerFn(value);
    isWaiting = true;
    setTimeout(processPending, delay);
  };
}

export function createDebouncedHandler<T>(
  handlerFn: (value: T) => Promise<void>,
  delay: number = 1000,
): (value: T) => Promise<void> {
  let timeoutId: ReturnType<typeof setTimeout> | null = null;

  return async function (value: T): Promise<void> {
    if (timeoutId) {
      // Clear the previous timeout if it exists
      clearTimeout(timeoutId);
    }

    timeoutId = setTimeout(async () => {
      await handlerFn(value);
      timeoutId = null;
    }, delay);
  };
}
