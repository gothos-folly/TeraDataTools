using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DataCenterUnpack
{
    // CFB in .net requires padding despite being a stream cipher. WTF.
    // from http://stackoverflow.com/a/29038974/445517
    public class NoPaddingTransformWrapper : ICryptoTransform
    {
        private ICryptoTransform m_Transform;

        public NoPaddingTransformWrapper(ICryptoTransform symmetricAlgoTransform)
        {
            if (symmetricAlgoTransform == null)
                throw new ArgumentNullException("symmetricAlgoTransform");

            m_Transform = symmetricAlgoTransform;
        }

        #region simple wrap

        public bool CanReuseTransform
        {
            get { return m_Transform.CanReuseTransform; }
        }

        public bool CanTransformMultipleBlocks
        {
            get { return m_Transform.CanTransformMultipleBlocks; }
        }

        public int InputBlockSize
        {
            get { return m_Transform.InputBlockSize; }
        }

        public int OutputBlockSize
        {
            get { return m_Transform.OutputBlockSize; }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            return m_Transform.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        #endregion

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount % m_Transform.InputBlockSize == 0)
                return m_Transform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
            else
            {
                byte[] lastBlocks = new byte[inputCount / m_Transform.InputBlockSize + m_Transform.InputBlockSize];
                Buffer.BlockCopy(inputBuffer, inputOffset, lastBlocks, 0, inputCount);
                byte[] result = m_Transform.TransformFinalBlock(lastBlocks, 0, lastBlocks.Length);
                Debug.Assert(inputCount < result.Length);
                Array.Resize(ref result, inputCount);
                return result;
            }
        }

        public void Dispose()
        {
            m_Transform.Dispose();
        }
    }
}