export type Page<T> = {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  items: T[];
};

export type Result<T, E = Error> =
  | { succeeded: true; failed: false; value: T }
  | { succeeded: false; failed: true; error: E };

export const result = Object.freeze({
  success<T>(value: T): Result<T> {
    return { succeeded: true, failed: false, value };
  },
  failure<E>(error: E): Result<never, E> {
    return { succeeded: false, failed: true, error };
  },
});
