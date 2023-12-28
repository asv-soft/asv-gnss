using System;
 using System.Collections.Generic;

 namespace Asv.Gnss
 {
     /// <summary>
     /// Factory to create and register ComNav binary messages.
     /// </summary>
     public static class ComNavBinaryFactory
     {
         /// <summary>
         /// Gets the default ComNav binary messages.
         /// </summary>
         public static IEnumerable<Func<ComNavBinaryMessageBase>> DefaultMessages
         {
             get { yield return () => new ComNavBinaryPsrPosPacket(); }
         }

         /// <summary>
         /// Registers the default messages in the specified ComNav binary parser.
         /// </summary>
         /// <param name="src">The ComNav binary parser where the default messages will be registered.</param>
         /// <returns>The ComNav binary parser with the default messages registered.</returns>
         public static ComNavBinaryParser RegisterDefaultMessages(this ComNavBinaryParser src)
         {
             foreach (var func in DefaultMessages)
             {
                 src.Register(func);
             }

             return src;
         }
     }
 }