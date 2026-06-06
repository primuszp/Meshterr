using OpenTK.Mathematics;

namespace Meshterr
{
    internal static class GLMath
    {
        public static bool UnProject(Vector3 window, double[] modelView, double[] projection, int[] viewport, out Vector3 world)
        {
            double[] transform = Multiply(modelView, projection);
            if (!Invert(transform, out double[] inverse))
            {
                world = Vector3.Zero;
                return false;
            }

            double x = (window.X - viewport[0]) / viewport[2] * 2.0 - 1.0;
            double y = (window.Y - viewport[1]) / viewport[3] * 2.0 - 1.0;
            double z = window.Z * 2.0 - 1.0;

            double[] input = { x, y, z, 1.0 };
            double[] output = MultiplyVector(inverse, input);

            if (output[3] == 0.0)
            {
                world = Vector3.Zero;
                return false;
            }

            world = new Vector3(
                (float)(output[0] / output[3]),
                (float)(output[1] / output[3]),
                (float)(output[2] / output[3]));
            return true;
        }

        private static double[] Multiply(double[] left, double[] right)
        {
            double[] result = new double[16];
            for (int column = 0; column < 4; column++)
            {
                for (int row = 0; row < 4; row++)
                {
                    result[column * 4 + row] =
                        left[0 * 4 + row] * right[column * 4 + 0] +
                        left[1 * 4 + row] * right[column * 4 + 1] +
                        left[2 * 4 + row] * right[column * 4 + 2] +
                        left[3 * 4 + row] * right[column * 4 + 3];
                }
            }

            return result;
        }

        private static double[] MultiplyVector(double[] matrix, double[] vector)
        {
            double[] result = new double[4];
            for (int row = 0; row < 4; row++)
            {
                result[row] =
                    matrix[0 * 4 + row] * vector[0] +
                    matrix[1 * 4 + row] * vector[1] +
                    matrix[2 * 4 + row] * vector[2] +
                    matrix[3 * 4 + row] * vector[3];
            }

            return result;
        }

        private static bool Invert(double[] matrix, out double[] inverse)
        {
            inverse = new double[16];

            inverse[0] = matrix[5] * matrix[10] * matrix[15] - matrix[5] * matrix[11] * matrix[14] - matrix[9] * matrix[6] * matrix[15] + matrix[9] * matrix[7] * matrix[14] + matrix[13] * matrix[6] * matrix[11] - matrix[13] * matrix[7] * matrix[10];
            inverse[4] = -matrix[4] * matrix[10] * matrix[15] + matrix[4] * matrix[11] * matrix[14] + matrix[8] * matrix[6] * matrix[15] - matrix[8] * matrix[7] * matrix[14] - matrix[12] * matrix[6] * matrix[11] + matrix[12] * matrix[7] * matrix[10];
            inverse[8] = matrix[4] * matrix[9] * matrix[15] - matrix[4] * matrix[11] * matrix[13] - matrix[8] * matrix[5] * matrix[15] + matrix[8] * matrix[7] * matrix[13] + matrix[12] * matrix[5] * matrix[11] - matrix[12] * matrix[7] * matrix[9];
            inverse[12] = -matrix[4] * matrix[9] * matrix[14] + matrix[4] * matrix[10] * matrix[13] + matrix[8] * matrix[5] * matrix[14] - matrix[8] * matrix[6] * matrix[13] - matrix[12] * matrix[5] * matrix[10] + matrix[12] * matrix[6] * matrix[9];
            inverse[1] = -matrix[1] * matrix[10] * matrix[15] + matrix[1] * matrix[11] * matrix[14] + matrix[9] * matrix[2] * matrix[15] - matrix[9] * matrix[3] * matrix[14] - matrix[13] * matrix[2] * matrix[11] + matrix[13] * matrix[3] * matrix[10];
            inverse[5] = matrix[0] * matrix[10] * matrix[15] - matrix[0] * matrix[11] * matrix[14] - matrix[8] * matrix[2] * matrix[15] + matrix[8] * matrix[3] * matrix[14] + matrix[12] * matrix[2] * matrix[11] - matrix[12] * matrix[3] * matrix[10];
            inverse[9] = -matrix[0] * matrix[9] * matrix[15] + matrix[0] * matrix[11] * matrix[13] + matrix[8] * matrix[1] * matrix[15] - matrix[8] * matrix[3] * matrix[13] - matrix[12] * matrix[1] * matrix[11] + matrix[12] * matrix[3] * matrix[9];
            inverse[13] = matrix[0] * matrix[9] * matrix[14] - matrix[0] * matrix[10] * matrix[13] - matrix[8] * matrix[1] * matrix[14] + matrix[8] * matrix[2] * matrix[13] + matrix[12] * matrix[1] * matrix[10] - matrix[12] * matrix[2] * matrix[9];
            inverse[2] = matrix[1] * matrix[6] * matrix[15] - matrix[1] * matrix[7] * matrix[14] - matrix[5] * matrix[2] * matrix[15] + matrix[5] * matrix[3] * matrix[14] + matrix[13] * matrix[2] * matrix[7] - matrix[13] * matrix[3] * matrix[6];
            inverse[6] = -matrix[0] * matrix[6] * matrix[15] + matrix[0] * matrix[7] * matrix[14] + matrix[4] * matrix[2] * matrix[15] - matrix[4] * matrix[3] * matrix[14] - matrix[12] * matrix[2] * matrix[7] + matrix[12] * matrix[3] * matrix[6];
            inverse[10] = matrix[0] * matrix[5] * matrix[15] - matrix[0] * matrix[7] * matrix[13] - matrix[4] * matrix[1] * matrix[15] + matrix[4] * matrix[3] * matrix[13] + matrix[12] * matrix[1] * matrix[7] - matrix[12] * matrix[3] * matrix[5];
            inverse[14] = -matrix[0] * matrix[5] * matrix[14] + matrix[0] * matrix[6] * matrix[13] + matrix[4] * matrix[1] * matrix[14] - matrix[4] * matrix[2] * matrix[13] - matrix[12] * matrix[1] * matrix[6] + matrix[12] * matrix[2] * matrix[5];
            inverse[3] = -matrix[1] * matrix[6] * matrix[11] + matrix[1] * matrix[7] * matrix[10] + matrix[5] * matrix[2] * matrix[11] - matrix[5] * matrix[3] * matrix[10] - matrix[9] * matrix[2] * matrix[7] + matrix[9] * matrix[3] * matrix[6];
            inverse[7] = matrix[0] * matrix[6] * matrix[11] - matrix[0] * matrix[7] * matrix[10] - matrix[4] * matrix[2] * matrix[11] + matrix[4] * matrix[3] * matrix[10] + matrix[8] * matrix[2] * matrix[7] - matrix[8] * matrix[3] * matrix[6];
            inverse[11] = -matrix[0] * matrix[5] * matrix[11] + matrix[0] * matrix[7] * matrix[9] + matrix[4] * matrix[1] * matrix[11] - matrix[4] * matrix[3] * matrix[9] - matrix[8] * matrix[1] * matrix[7] + matrix[8] * matrix[3] * matrix[5];
            inverse[15] = matrix[0] * matrix[5] * matrix[10] - matrix[0] * matrix[6] * matrix[9] - matrix[4] * matrix[1] * matrix[10] + matrix[4] * matrix[2] * matrix[9] + matrix[8] * matrix[1] * matrix[6] - matrix[8] * matrix[2] * matrix[5];

            double determinant = matrix[0] * inverse[0] + matrix[1] * inverse[4] + matrix[2] * inverse[8] + matrix[3] * inverse[12];
            if (determinant == 0.0)
            {
                return false;
            }

            determinant = 1.0 / determinant;
            for (int i = 0; i < 16; i++)
            {
                inverse[i] *= determinant;
            }

            return true;
        }
    }
}
